namespace DCFApixels.DragonECS
{
    public static class EcsGroupExtensions
    {
        public static void IterateParallel(this EcsGroup self, EcsThreadHandler worker, int minSpanSize)
        {
            IterateParallel(self, worker, minSpanSize);
        }
        public static void IterateParallel(this EcsReadonlyGroup self, EcsThreadHandler worker, int minSpanSize)
        {
            ThreadRunner.Run(worker, self, minSpanSize);
        }
    }
}
