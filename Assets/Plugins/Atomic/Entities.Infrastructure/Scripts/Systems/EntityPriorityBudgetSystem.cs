using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityPriorityBudgetSystem<E> : PriorityBudgetSystem<E> where E : IEntity
    {
        protected EntityPriorityBudgetSystem(
            IReadOnlyStore<E> source,
            int budget = DEFAULT_BUDGET,
            float priorityCooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, budget, priorityCooldown, trackers)
        {
        }
    }
}