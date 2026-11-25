using Atomic.Entities;
using Atomic.Infrastructure;

namespace RTSGame
{
    public abstract class EntityBudgetGameSystem : EntityBudgetSystem<IGameEntity>
    {
        protected EntityBudgetGameSystem(
            IReadOnlyStore<IGameEntity> source,
            int budget = DEFAULT_BUDGET
        ) : base(source, budget)
        {
        }
    }
}