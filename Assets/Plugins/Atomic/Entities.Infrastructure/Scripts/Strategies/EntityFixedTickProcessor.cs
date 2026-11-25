using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityFixedTickProcessor : EntityTickProcessor<IEntity>
    {
    }

    public class EntityFixedTickProcessor<E> : IProcessor<E> where E : IEntity
    {
        public static readonly EntityTickProcessor<E> Default = new();

        public void Process(E obj, float deltaTime) => obj.FixedTick(deltaTime);
    }
}