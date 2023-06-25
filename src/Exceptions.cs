using System;

namespace DCFApixels.DragonECS
{
    public static class EcsThrowHalper_ClassicThreads
    {
        internal static void DoubleParallelIteration(this EcsThrowHalper _)
        {
            throw new InvalidOperationException("It is forbidden to start a parallel iteration before the last one is finished.");
        }
    }
}
