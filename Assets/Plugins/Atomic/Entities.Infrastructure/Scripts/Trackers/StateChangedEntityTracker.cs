namespace Atomic.Entities
{
    /// <summary>
    /// A non-generic shortcut for <see cref="StateChangedEntityTracker{E}"/>.
    /// Subscribes to state change events (<c>OnStateChanged</c>) on basic <see cref="IEntity"/> instances.
    /// </summary>
    public class StateChangedEntityTracker : StateChangedEntityTracker<IEntity>
    {
    }
}