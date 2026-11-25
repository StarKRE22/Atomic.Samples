using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityPriorityBudgetSystem<E> : PriorityBudgetSystem<E> where E : IEntity
    {
        protected EntityPriorityBudgetSystem(
            IReadOnlyStore<E> source,
            IUpdater<E> updater,
            IUpdatePriorityStrategy<E> priorityStrategy,
            int budget = DEFAULT_BUDGET,
            float priorityCooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, updater, priorityStrategy, budget, priorityCooldown, trackers)
        {
        }
    }
}