using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityLateTickJob : EntityTickJob<IEntity>
    {
    }

    public class EntityLateTickJob<E> : IJob<E> where E : IEntity
    {
        public static readonly EntityTickJob<E> Default = new();

        public void Execute(E obj, float deltaTime) => obj.LateTick(deltaTime);
    }
}