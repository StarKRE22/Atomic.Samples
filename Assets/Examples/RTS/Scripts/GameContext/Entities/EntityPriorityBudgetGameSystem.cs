using Atomic.Entities;
using Atomic.Infrastructure;

namespace RTSGame
{
    public abstract class EntityPriorityBudgetGameSystem : EntityPriorityBudgetSystem<IGameEntity>
    {
        protected EntityPriorityBudgetGameSystem(
            IReadOnlyEntityCollection<IGameEntity> source,
            int budget = DEFAULT_BUDGET,
            float priorityCooldown = DEFAULT_PRIORITY_COOLDOWN
        ) : base(source, budget, priorityCooldown)
        {
        }

        protected override UpdatePriority EvaluatePriority(IGameEntity entity) =>
            entity.GetUpdatePriority().Value;
    }
}