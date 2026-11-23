namespace Atomic.Entities
{
    /// <summary>
    /// Represents a trigger that reacts to an <see cref="IEntity"/> interaction or condition.
    /// This is a shorthand for <see cref="IEntityTracker{E}"/> with the default entity type.
    /// </summary>
    public interface IEntityTracker : IEntityTracker<IEntity>
    {
    }
}