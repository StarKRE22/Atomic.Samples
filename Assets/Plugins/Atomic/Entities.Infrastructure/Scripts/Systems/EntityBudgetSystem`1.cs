using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityBudgetSystem<E> : BudgetSystem<E> where E : IEntity
    {
        public EntityBudgetSystem(
            IReadOnlyStore<E> source,
            IUpdater<E> updater,
            int budget = DEFAULT_BUDGET
        ) : base(source, updater, budget)
        {
        }
    }
}