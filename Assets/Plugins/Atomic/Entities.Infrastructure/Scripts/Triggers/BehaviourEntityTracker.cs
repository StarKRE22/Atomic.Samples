namespace Atomic.Entities
{
    /// <summary>
    /// A non-generic shortcut for <see cref="BehaviourEntityTracker{E}"/>.
    /// Subscribes to behaviour-related events (<c>OnBehaviourAdded</c>, <c>OnBehaviourRemoved</c>) on basic <see cref="IEntity"/> instances.
    /// </summary>
    public class BehaviourEntityTracker : BehaviourEntityTracker<IEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourEntityTracker"/> class.
        /// </summary>
        /// <param name="added">Whether to react to behaviour additions via <c>OnBehaviourAdded</c>.</param>
        /// <param name="removed">Whether to react to behaviour removals via <c>OnBehaviourRemoved</c>.</param>
        public BehaviourEntityTracker(bool added = true, bool removed = true) : base(added, removed)
        {
        }
    }
}