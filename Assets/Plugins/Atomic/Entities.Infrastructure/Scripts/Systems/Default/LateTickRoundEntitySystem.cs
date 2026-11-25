using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public sealed class LateTickRoundEntitySystem<E> : RoundEntitySystem<E> where E : IEntity
    {
        public LateTickRoundEntitySystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.LateTick(deltaTime);
    }
}