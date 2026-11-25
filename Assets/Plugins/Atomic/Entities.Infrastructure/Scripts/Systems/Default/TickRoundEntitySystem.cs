using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public sealed class TickRoundEntitySystem<E> : RoundEntitySystem<E> where E : IEntity
    {
        public TickRoundEntitySystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) => 
            entity.Tick(deltaTime);
    }
}