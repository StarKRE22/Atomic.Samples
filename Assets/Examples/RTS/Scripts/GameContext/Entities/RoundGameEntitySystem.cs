using Atomic.Entities;
using Atomic.Infrastructure;

namespace RTSGame
{
    public abstract class RoundGameEntitySystem : RoundEntitySystem<IGameEntity>
    {
        protected RoundGameEntitySystem(
            IReadOnlyStore<IGameEntity> source,
            int budget = DEFAULT_BUDGET
        ) : base(source, budget)
        {
        }
    }
}