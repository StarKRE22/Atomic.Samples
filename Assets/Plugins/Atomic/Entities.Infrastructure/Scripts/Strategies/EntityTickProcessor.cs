using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityTickProcessor : EntityTickProcessor<IEntity>
    {
    }

    public class EntityTickProcessor<E> : IProcessor<E> where E : IEntity
    {
        public static readonly EntityTickProcessor<E> Default = new();

        public void Process(E obj, float deltaTime) => obj.Tick(deltaTime);
    }
}