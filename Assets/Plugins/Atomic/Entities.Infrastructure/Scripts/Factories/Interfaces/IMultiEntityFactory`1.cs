using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Defines a factory for creating and managing multiple entities identified by a key of type <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of key used to identify an entity.
    /// </typeparam>
    /// <typeparam name="E">
    /// The type of entity to be created, which must implement the <see cref="IEntity"/> interface.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of construction arguments passed to the factory. Must implement <see cref="IArgs"/>.
    /// </typeparam>
    public interface IMultiEntityFactory<in TKey, E, in TArgs> : IMultiFactory<TKey, E, TArgs>
        where TArgs : IArgs
        where E : IEntity
    {
    }
}