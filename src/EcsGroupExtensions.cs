namespace DCFApixels.DragonECS
{
    public static class EcsGroupExtensions
    {
        public static void IterateParallel(this EcsGroup self, ThreadWorkerHandler worker, int minSpanSize)
        {
            IterateParallel(self, worker, minSpanSize);
        }
        public static void IterateParallel(this EcsReadonlyGroup self, ThreadWorkerHandler worker, int minSpanSize)
        {
            ThreadRunner.Run(worker, self, minSpanSize);
        }
    }
}
