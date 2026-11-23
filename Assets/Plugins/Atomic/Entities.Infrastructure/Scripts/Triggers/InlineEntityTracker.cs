using System;

namespace Atomic.Entities
{
    /// <summary>
    /// A non-generic shortcut for <see cref="InlineEntityTracker{E}"/>.
    /// Provides inline tracking logic for basic <see cref="IEntity"/> instances.
    /// </summary>
    public class InlineEntityTracker : InlineEntityTracker<IEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineEntityTracker"/> class
        /// with the specified tracking and untracking delegates.
        /// </summary>
        /// <param name="track">A delegate that defines how to start tracking the entity, given an action.</param>
        /// <param name="untrack">A delegate that defines how to stop tracking the entity, given an action.</param>
        public InlineEntityTracker(
            Action<IEntity, Action<IEntity>> track,
            Action<IEntity, Action<IEntity>> untrack
        ) : base(track, untrack)
        {
        }
    }
}