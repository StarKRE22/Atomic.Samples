using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityTickStrategy : EntityTickStrategy<IEntity>
    {
    }

    public class EntityTickStrategy<E> : IUpdateStrategy<E> where E : IEntity
    {
        public static readonly EntityTickStrategy<E> Default = new();

        public void Update(E obj, float deltaTime) => obj.Tick(deltaTime);
    }
}