using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityBudgetSystem : BudgetSystem<IEntity>
    {
        protected EntityBudgetSystem(
            IReadOnlyStore<IEntity> source,
            IUpdateStrategy<IEntity> updateStrategy,
            int budget = DEFAULT_BUDGET
        ) : base(source, updateStrategy, budget)
        {
        }
    }
}