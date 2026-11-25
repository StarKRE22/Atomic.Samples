using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityFixedTickStrategy : EntityTickStrategy<IEntity>
    {
    }

    public class EntityFixedTickStrategy<E> : IUpdateStrategy<E> where E : IEntity
    {
        public static readonly EntityTickStrategy<E> Default = new();

        public void Update(E obj, float deltaTime) => obj.FixedTick(deltaTime);
    }
}