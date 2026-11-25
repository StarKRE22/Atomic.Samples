using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityLateTickProcessor : EntityTickProcessor<IEntity>
    {
    }

    public class EntityLateTickProcessor<E> : IProcessor<E> where E : IEntity
    {
        public static readonly EntityTickProcessor<E> Default = new();

        public void Process(E obj, float deltaTime) => obj.LateTick(deltaTime);
    }
}