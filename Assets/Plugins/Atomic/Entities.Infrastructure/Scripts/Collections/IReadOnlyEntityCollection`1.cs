using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a read-only, observable collection of entities of type <typeparamref name="E"/>.
    /// Provides access to entity presence, enumeration, and change events.
    /// </summary>
    /// <typeparam name="E">The type of entity contained in the collection. Must implement <see cref="IEntity"/>.</typeparam>
    public interface IReadOnlyEntityCollection<E> : IReadOnlyStore<E> where E : IEntity
    {
    }
}