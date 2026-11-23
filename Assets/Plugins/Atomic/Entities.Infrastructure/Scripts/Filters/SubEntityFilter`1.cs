using System;
using Atomic.Infrastructure;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a dynamic, type-safe view (filter) over an existing entity collection,
    /// selecting entities of type <typeparamref name="T"/> from a source of type <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="T">The derived entity type to include in the filter.</typeparam>
    /// <typeparam name="E">The base entity type that the source collection exposes.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="SubEntityFilter{T,E}"/> observes a source collection of entities
    /// and maintains a live subset of entities that match the given <paramref name="predicate"/>.
    /// </para>
    /// <para>
    /// The filter automatically synchronizes its internal state whenever entities are
    /// added to or removed from the source, or when tracked entities trigger state changes
    /// via <see cref="IEntityTracker{E}"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="IReadOnlyEntityCollection{TEntity}"/>
    /// <seealso cref="IEntityTracker{E}"/>
    /// <seealso cref="EntityCollection{TEntity}"/>
    public class SubEntityFilter<T, E> : SubObjectFilter<T, E>, IReadOnlyEntityCollection<T>
        where E : IEntity
        where T : E
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilter{E}"/> class.
        /// </summary>
        /// <param name="source">The source entity collection to observe.</param>
        /// <param name="predicate">The predicate used to determine filter inclusion.</param>
        /// <param name="triggers">Optional triggers used to re-evaluate entities when their state changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public SubEntityFilter(
            IReadOnlyEntityCollection<E> source,
            Predicate<T> predicate,
            params ITracker<T>[] triggers
        ) : base(source, predicate, triggers)
        {
        }
    }
}