using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityLateTickStrategy : EntityTickStrategy<IEntity>
    {
    }

    public class EntityLateTickStrategy<E> : IUpdateStrategy<E> where E : IEntity
    {
        public static readonly EntityTickStrategy<E> Default = new();

        public void Update(E obj, float deltaTime) => obj.LateTick(deltaTime);
    }
}