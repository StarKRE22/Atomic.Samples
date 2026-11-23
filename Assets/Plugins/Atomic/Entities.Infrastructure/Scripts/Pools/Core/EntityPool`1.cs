using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// A simple object pool for entities of type <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">The entity type managed by the pool. Must implement <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TArgs">The type of construction arguments passed to the entity factory.</typeparam>
    /// <remarks>
    /// This pool creates entities using an <see cref="IEntityFactory{E}"/> and supports reuse through
    /// <see cref="Rent"/> and <see cref="Return(E)"/> methods.
    /// It also provides virtual lifecycle hooks for spawn, rent, return, and despawn operations.
    /// </remarks>
    public class EntityPool<E, TArgs> : Pool<E, TArgs>, IEntityPool<E>
        where E : IEntity
        where TArgs : IArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPool{E}"/> class using the specified factory.
        /// </summary>
        /// <param name="factory">The factory used to create new entity instances when needed.</param>
        /// <param name="args">The arguments passed to the factory when creating entities.</param>
        /// <param name="expandMode">Determines how the pool expands when empty. Defaults to <see cref="ExpandMode.ExpandByOne"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> or <paramref name="args"/> is null.</exception>
        public EntityPool(IEntityFactory<E, TArgs> factory, TArgs args,
            ExpandMode expandMode = ExpandMode.ExpandByOne) : base(factory, args, expandMode)
        {
        }
    }
}