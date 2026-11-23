using System;
using Atomic.Infrastructure;


namespace Atomic.Entities
{
    /// <summary>
    /// Represents a dynamic, observable filtered view over an existing <see cref="IReadOnlyEntityCollection{E}"/>.
    /// Entities are included based on a predicate and tracked using optional triggers.
    /// </summary>
    /// <typeparam name="E">The type of entity being filtered. Must implement <see cref="IEntity"/>.</typeparam>
    public class EntityFilter<E> : ObjectFilter<E>, IReadOnlyEntityCollection<E> where E : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilter{E}"/> class.
        /// </summary>
        /// <param name="source">The source entity collection to observe.</param>
        /// <param name="predicate">The predicate used to determine filter inclusion.</param>
        /// <param name="triggers">Optional triggers used to re-evaluate entities when their state changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public EntityFilter(
            IReadOnlyEntityCollection<E> source,
            Predicate<E> predicate,
            params ITracker<E>[] triggers
        ) : base(source, predicate, triggers)
        {
        }
    }
}