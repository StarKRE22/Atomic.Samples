using System;
using Atomic.Infrastructure;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// A registry that manages multiple pools of entities, each identified by a unique key.
    /// </summary>
    /// <typeparam name="TKey">The key type used to identify each pool.</typeparam>
    /// <typeparam name="E">The entity type managed by the pools. Must implement <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TArgs">The type of construction arguments passed to the entity factory.</typeparam>
    public class MultiEntityPool<TKey, E, TArgs> : MultiPool<TKey, E, TArgs>, IMultiEntityPool<TKey, E>
        where E : IEntity
        where TArgs : IArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiEntityPool{TKey,E}"/> class.
        /// </summary>
        /// <param name="factory">The factory registry used to create entities for each key.</param>
        /// <param name="args">The arguments passed to the factory when creating entities.</param>
        /// <param name="expandMode">Determines how the pool expands when empty. Defaults to <see cref="ExpandMode.ExpandByOne"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is null.</exception>
        public MultiEntityPool(IMultiEntityFactory<TKey, E, TArgs> factory, TArgs args,
            ExpandMode expandMode = ExpandMode.ExpandByOne) : base(factory, args, expandMode)
        {
        }
    }
}