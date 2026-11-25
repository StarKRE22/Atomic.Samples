using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class LateTickEntityPriorityBudgetSystem<E> : EntityPriorityBudgetSystem<E> where E : IEntity
    {
        protected LateTickEntityPriorityBudgetSystem(
            IReadOnlyStore<E> source,
            int budget = DEFAULT_BUDGET
        ) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.LateTick(deltaTime);
    }
}