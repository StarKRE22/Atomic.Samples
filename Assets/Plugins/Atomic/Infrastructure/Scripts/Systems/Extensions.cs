namespace Atomic.Infrastructure
{
    public static partial class Extensions
    {
        public static AdaptiveBudgetSystem AsAdaptive(
            this IBudgetSystem it,
            IBudgetStrategy strategy,
            float updateInterval = AdaptiveBudgetSystem.DEFAULT_INTERVAL
        ) => new(it, strategy, updateInterval);
    }
}