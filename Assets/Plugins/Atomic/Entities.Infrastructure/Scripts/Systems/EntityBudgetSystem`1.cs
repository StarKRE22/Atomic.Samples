using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityBudgetSystem<E> : BudgetSystem<E> where E : IEntity
    {
        protected EntityBudgetSystem(
            IReadOnlyStore<E> source,
            IUpdateStrategy<E> updateStrategy,
            int budget = DEFAULT_BUDGET
        ) : base(source, updateStrategy, budget)
        {
        }
    }
}