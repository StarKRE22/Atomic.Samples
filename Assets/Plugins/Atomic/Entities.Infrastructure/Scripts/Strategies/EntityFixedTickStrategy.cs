using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityFixedTickUpdater : EntityTickUpdater<IEntity>
    {
    }

    public class EntityFixedTickUpdater<E> : IUpdater<E> where E : IEntity
    {
        public static readonly EntityTickUpdater<E> Default = new();

        public void Update(E obj, float deltaTime) => obj.FixedTick(deltaTime);
    }
}