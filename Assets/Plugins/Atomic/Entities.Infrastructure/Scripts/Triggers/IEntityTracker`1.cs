using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a trigger that monitors specific aspects of an entity's state
    /// and signals when the entity should be re-evaluated by a filter.
    /// </summary>
    /// <typeparam name="E">The entity type being tracked.</typeparam>
    public interface IEntityTracker<E> : ITracker<E> where E : IEntity
    {
    }
}