using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class TickEntityPriorityBudgetSystem<E> : EntityPriorityBudgetSystem<E> where E : IEntity
    {
        protected TickEntityPriorityBudgetSystem(
            IReadOnlyStore<E> source,
            int budget = DEFAULT_BUDGET
        ) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.Tick(deltaTime);
    }
}