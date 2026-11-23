using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Atomic.Infrastructure
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a range of entities to the collection.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="it">The target entity collection.</param>
        /// <param name="entities">The array of entities to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this IStore<T> it, params T[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            for (int i = 0, count = entities.Length; i < count; i++)
                it.Add(entities[i]);
        }

        /// <summary>
        /// Adds a range of entities from an enumerable to the collection.
        /// Ignores null entities.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="it">The target entity collection.</param>
        /// <param name="entities">The enumerable of entities to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<E>(this IStore<E> it, IEnumerable<E> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (E entity in entities)
                it.Add(entity);
        }
    }
}