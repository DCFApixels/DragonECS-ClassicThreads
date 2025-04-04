﻿#if DISABLE_DEBUG
#undef DEBUG
#endif

namespace DCFApixels.DragonECS
{
    public static class EcsCollectionsExtensions
    {
        public static void IterateParallel(this EcsGroup self, EcsThreadHandler worker, int minSpanSize)
        {
            ThreadRunner.Run(worker, self.ToSpan(), minSpanSize);
        }
        public static void IterateParallel(this EcsReadonlyGroup self, EcsThreadHandler worker, int minSpanSize)
        {
            ThreadRunner.Run(worker, self.ToSpan(), minSpanSize);
        }
        public static void IterateParallel(this EcsSpan self, EcsThreadHandler worker, int minSpanSize)
        {
            ThreadRunner.Run(worker, self, minSpanSize);
        }
    }
}
