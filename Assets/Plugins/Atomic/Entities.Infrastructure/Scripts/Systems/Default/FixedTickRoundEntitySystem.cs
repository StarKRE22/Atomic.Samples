using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public sealed class FixedTickRoundEntitySystem<E> : RoundEntitySystem<E> where E : IEntity
    {
        public FixedTickRoundEntitySystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) => 
            entity.FixedTick(deltaTime);
    }
}