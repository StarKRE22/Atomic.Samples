using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityBudgetSystem : BudgetSystem<IEntity>
    {
        public EntityBudgetSystem(
            IReadOnlyStore<IEntity> source,
            IUpdater<IEntity> updater,
            int budget = DEFAULT_BUDGET
        ) : base(source, updater, budget)
        {
        }
    }
}