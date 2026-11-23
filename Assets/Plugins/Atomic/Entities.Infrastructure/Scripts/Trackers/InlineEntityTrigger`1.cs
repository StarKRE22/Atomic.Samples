using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// An inline-configurable entity trigger that allows custom tracking and untracking logic
    /// for a specific entity type.
    /// </summary>
    /// <typeparam name="E">The type of entity to track, constrained to <see cref="IEntity"/>.</typeparam>
    public class InlineEntityTracker<E> : InlineTracker<E>, IEntityTracker<E> where E : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineEntityTracker{E}"/> class
        /// with the specified tracking and untracking logic.
        /// </summary>
        /// <param name="track">
        /// A delegate that receives the entity and the trigger action,
        /// used to define how the entity should be subscribed or monitored.
        /// </param>
        /// <param name="untrack">
        /// A delegate that receives the entity and the trigger action,
        /// used to define how the entity should be unsubscribed or ignored.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="track"/> or <paramref name="untrack"/> is null.</exception>
        public InlineEntityTracker(Action<E, Action<E>> track, Action<E, Action<E>> untrack) : base(track, untrack)
        {
        }
    }
}