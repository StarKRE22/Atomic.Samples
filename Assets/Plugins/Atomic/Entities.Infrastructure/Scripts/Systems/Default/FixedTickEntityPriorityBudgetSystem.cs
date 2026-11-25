using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class FixedTickEntityPriorityBudgetSystem<E> : EntityPriorityBudgetSystem<E> where E : IEntity
    {
        protected FixedTickEntityPriorityBudgetSystem(
            IReadOnlyStore<E> source,
            int budget = DEFAULT_BUDGET
        ) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.FixedTick(deltaTime);
    }
}