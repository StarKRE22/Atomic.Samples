namespace Atomic.Infrastructure
{
    public interface IMultiFactory<in TKey, T, in TArgs> where TArgs : IArgs
    {
        /// <summary>
        /// Creates a new entity based on the specified key.
        /// </summary>
        /// <param name="key">The key used to create the entity.</param>
        /// <param name="args"></param>
        /// <returns>The newly created entity of type <typeparamref name="T"/>.</returns>
        T Create(TKey key, TArgs args);

        /// <summary>
        /// Attempts to create an entity based on the specified key.
        /// </summary>
        /// <param name="key">The key used to create the entity.</param>
        /// <param name="args"></param>
        /// <param name="entity">
        ///     When this method returns, contains the created entity if the operation succeeded; otherwise, the default value of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the entity was successfully created; 
        /// <c>false</c> if creation failed (e.g., an entity with the same key already exists or the key is invalid).
        /// </returns>
        bool TryCreate(TKey key, TArgs args, out T entity);

        /// <summary>
        /// Checks whether an entity with the specified key exists.
        /// </summary>
        /// <param name="key">The key of the entity to check for existence.</param>
        /// <returns>
        /// <c>true</c> if an entity with the given key exists; 
        /// <c>false</c> otherwise.
        /// </returns>
        bool Contains(TKey key);
    }
}