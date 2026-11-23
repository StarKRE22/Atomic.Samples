using UnityEngine;

namespace Atomic.Infrastructure
{
    public abstract class ScriptableAspect<T> : ScriptableObject, IAspect<T>
    {
        /// <summary>
        /// Applies this aspect to the specified entity.
        /// </summary>
        /// <param name="entity">The entity to which the aspect should be applied.</param>
        public abstract void Apply(T entity);

        /// <summary>
        /// Discards this aspect from the specified entity, reversing any changes made by <see cref="Apply"/>.
        /// </summary>
        /// <param name="entity">The entity from which the aspect should be removed.</param>
        public abstract void Discard(T entity);
    }
}