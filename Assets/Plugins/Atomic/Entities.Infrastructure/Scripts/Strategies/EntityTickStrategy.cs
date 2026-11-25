using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityTickUpdater : EntityTickUpdater<IEntity>
    {
    }

    public class EntityTickUpdater<E> : IUpdater<E> where E : IEntity
    {
        public static readonly EntityTickUpdater<E> Default = new();

        public void Update(E obj, float deltaTime) => obj.Tick(deltaTime);
    }
}