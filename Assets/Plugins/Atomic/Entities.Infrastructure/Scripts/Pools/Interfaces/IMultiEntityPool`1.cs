using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a registry that manages multiple entity pools, each identified by a key.
    /// </summary>
    /// <typeparam name="TKey">The key type used to identify individual pools.</typeparam>
    /// <typeparam name="E">The type of entity managed by the pools. Must implement <see cref="IEntity"/>.</typeparam>
    public interface IMultiEntityPool<in TKey, E> : IMultiPool<TKey, E> where E : IEntity
    {
    }
}