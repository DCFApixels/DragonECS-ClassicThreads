using System;
using System.Threading;

namespace DCFApixels.DragonECS
{
    internal static class ThreadRunner
    {
        private readonly static int _maxThreadsCount;
        private static ThreadReacord[] _threads;

        private static ThreadWorkerHandler _worker;
        private static ThreadWorkerHandler _nullWorker = delegate { };
        private static int[] _entities = new int[64];
        private static List<Exception> _catchedExceptions;

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
                    if (_catchedExceptions == null)
                        _catchedExceptions = new List<Exception>();
                    _catchedExceptions.Add(e);
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

        public static void Run(ThreadWorkerHandler worker, EcsReadonlyGroup entities, int minSpanSize)
        {
            _worker = worker;
            int entitiesCount = entities.Bake(ref _entities);

            int threadsCount = entitiesCount / minSpanSize;
            if (entitiesCount % minSpanSize > 0)
                threadsCount++;
            if (threadsCount > _maxThreadsCount)
                threadsCount = _maxThreadsCount;

            if (threadsCount > 1)
            {
                int spanSize = entitiesCount / (threadsCount - 1);
                for (int i = 0; i < threadsCount; i++)
                {
                    ref var thread = ref _threads[i];
                    thread.start = i * spanSize;
                    thread.size = spanSize;
                }
                _threads[threadsCount - 1].size = entities.Count % (threadsCount - 1);
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

            _worker = _nullWorker;
            if(_catchedExceptions != null)
            {
                var exceptions = _catchedExceptions;
                _catchedExceptions = null;
                throw new AggregateException("Mutiplie exceptions", exceptions);
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
    public delegate void ThreadWorkerHandler(ReadOnlySpan<int> entities);
}