namespace Atomic.Entities
{
    /// <summary>
    /// A non-generic shortcut for <see cref="TagEntityTracker{E}"/>.
    /// Subscribes to tag-related events (<c>OnTagAdded</c>, <c>OnTagDeleted</c>) on basic <see cref="IEntity"/> instances.
    /// </summary>
    public class TagEntityTracker : TagEntityTracker<IEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagEntityTracker"/> class.
        /// </summary>
        /// <param name="added">Whether to react to tag additions via <c>OnTagAdded</c>.</param>
        /// <param name="deleted">Whether to react to tag removals via <c>OnTagDeleted</c>.</param>
        public TagEntityTracker(bool added = true, bool deleted = true) : base(added, deleted)
        {
        }
    }
}