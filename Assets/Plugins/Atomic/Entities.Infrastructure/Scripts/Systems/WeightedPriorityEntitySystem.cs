using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class WeightedPriorityEntitySystem<E> : WeightedPrioritySystem<E>
    {
        protected WeightedPriorityEntitySystem(
            IReadOnlyStore<E> source,
            Distribution distribution,
            int budget = DEFAULT_BUDGET,
            float cooldown = DEFAULT_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, distribution, budget, cooldown, trackers)
        {
        }
    }
}