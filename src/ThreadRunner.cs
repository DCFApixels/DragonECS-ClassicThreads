using DCFApixels.DragonECS.ClassicThreadsInternal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DCFApixels.DragonECS
{
    internal static class ThreadRunner
    {
        private readonly static int _maxThreadsCount;
        private static ThreadReacord[] _threads;

        private static EcsThreadHandler _worker;
        private static EcsThreadHandler _nullWorker = delegate { };
        private static int[] _entities = new int[64];
        private static ConcurrentQueue<Exception> _catchedExceptions = new ConcurrentQueue<Exception>();

        private static bool _isRunning = false;

        private static void ThreadProc(object obj)
        {
            int i = (int)obj;
            ref ThreadReacord record = ref _threads[i];

            while (Thread.CurrentThread.IsAlive)
            {
                try
                {
                    record.runWork.WaitOne();
                    record.runWork.Reset();
                    _worker.Invoke(new ReadOnlySpan<int>(_entities, record.start, record.size));
                    record.doneWork.Set();
                }
                catch (Exception e)
                {
                    _catchedExceptions.Enqueue(e);
                    record.doneWork.Set();
                }
            }
        }

        static ThreadRunner()
        {
            _maxThreadsCount = Environment.ProcessorCount;
            _threads = new ThreadReacord[_maxThreadsCount];

            for (int i = 0; i < _maxThreadsCount; i++)
            {
                _threads[i] = new ThreadReacord()
                {
                    thread = new Thread(ThreadProc) { IsBackground = true },
                    runWork = new ManualResetEvent(false),
                    doneWork = new ManualResetEvent(true),
                };
                _threads[i].thread.Start(i);
            }
            _worker = _nullWorker;
        }

        public static void Run(EcsThreadHandler worker, EcsSpan entities, int minSpanSize)
        {
#if (DEBUG && !DISABLE_DEBUG) || ENABLE_DRAGONECS_ASSERT_CHEKS
            if (_isRunning) Throw.DoubleParallelIteration();
#endif
            _isRunning = true;
            _worker = worker;
            int entitiesCount = entities.Length; //entities.Bake(ref _entities);

            int threadsCount = entitiesCount / minSpanSize;
            if (entitiesCount % minSpanSize > 0)
            {
                threadsCount++;
            }
            if (threadsCount > _maxThreadsCount)
            {
                threadsCount = _maxThreadsCount;
            }

            if (threadsCount > 1)
            {
                int remainder = entitiesCount % threadsCount;
                int quotient = entitiesCount / threadsCount;
                for (int i = 0, start = 0; i < threadsCount; i++)
                {
                    ref var thread = ref _threads[i];
                    thread.start = start;
                    thread.size = quotient;
                    if (remainder > 0)
                    {
                        thread.size++;
                        remainder--;
                    }
                    start += thread.size;
                }
            }
            else
            {
                threadsCount = 1;
                ref var thread = ref _threads[0];
                thread.start = 0;
                thread.size = entitiesCount;
            }

            for (int i = 0; i < threadsCount; i++)
            {
                ref var thread = ref _threads[i];
                thread.doneWork.Reset();
                thread.runWork.Set();
            }
            for (int i = 0; i < threadsCount; i++)
            {
                _threads[i].doneWork.WaitOne();
            }

            _isRunning = false;
            _worker = _nullWorker;
            if (_catchedExceptions.Count > 0)
            {
                Exception[] exceptions = _catchedExceptions.ToArray();
                _catchedExceptions.Clear();
                throw new AggregateException(exceptions);
            }
        }

        private struct ThreadReacord
        {
            public Thread thread;
            public ManualResetEvent runWork;
            public ManualResetEvent doneWork;
            public int start;
            public int size;
        }
    }
    public delegate void EcsThreadHandler(ReadOnlySpan<int> entities);
}