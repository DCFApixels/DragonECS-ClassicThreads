using System;

namespace DCFApixels.DragonECS
{
    namespace ClassicThreadsInternal
    {
        internal static class Throw
        {
            internal static void DoubleParallelIteration()
            {
                throw new InvalidOperationException("It is forbidden to start a parallel iteration before the last one is finished.");
            }
        }
    }
}
