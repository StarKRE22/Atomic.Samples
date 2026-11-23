using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a pool for reusing <see cref="IEntity"/> instances to reduce allocation overhead.
    /// </summary>
    public interface IEntityPool<E> : IPool<E> where E : IEntity
    {
    }
}