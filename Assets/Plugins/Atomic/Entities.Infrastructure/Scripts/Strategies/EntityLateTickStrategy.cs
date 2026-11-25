using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityLateTickUpdater : EntityTickUpdater<IEntity>
    {
    }

    public class EntityLateTickUpdater<E> : IUpdater<E> where E : IEntity
    {
        public static readonly EntityTickUpdater<E> Default = new();

        public void Update(E obj, float deltaTime) => obj.LateTick(deltaTime);
    }
}