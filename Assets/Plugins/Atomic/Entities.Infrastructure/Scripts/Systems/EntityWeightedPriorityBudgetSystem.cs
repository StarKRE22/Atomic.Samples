using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityWeightedPriorityBudgetSystem<E> : WeightedPriorityBudgetSystem<E> where E : IEntity
    {
        protected EntityWeightedPriorityBudgetSystem(
            IReadOnlyStore<E> source,
            Distribution distribution,
            int budget = DEFAULT_BUDGET,
            float priorityCooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, distribution, budget, priorityCooldown, trackers)
        {
        }
    }
}