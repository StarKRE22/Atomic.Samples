using Atomic.Entities;
using Atomic.Infrastructure;

namespace RTSGame
{
    public abstract class PriorityGameEntitySystem : PriorityEntitySystem<IGameEntity>
    {
        protected PriorityGameEntitySystem(
            IReadOnlyEntityCollection<IGameEntity> source,
            int budget = DEFAULT_BUDGET,
            float cooldown = DEFAULT_COOLDOWN
        ) : base(source, budget, cooldown)
        {
        }

        protected override UpdatePriority EvaluatePriority(IGameEntity entity) =>
            entity.GetUpdatePriority().Value;
    }
}