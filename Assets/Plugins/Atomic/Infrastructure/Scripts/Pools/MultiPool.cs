using System;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    /// <summary>
    /// A registry that manages multiple pools of entities, each identified by a unique key.
    /// </summary>
    /// <typeparam name="TKey">The key type used to identify each pool.</typeparam>
    /// <typeparam name="T">The entity type managed by the pools. Must implement <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TArgs">The type of construction arguments passed to the entity factory.</typeparam>
    public class MultiPool<TKey, T, TArgs> : IMultiPool<TKey, T> where TArgs : IArgs
    {
        /// <summary>
        /// Internal storage of pooled (available) entities, mapped by their pool key.
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly Dictionary<TKey, Stack<T>> _pooledEntities = new();

        /// <summary>
        /// Tracks entities that are currently rented, mapping them back to their original pool key.
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly Dictionary<T, TKey> _rentEntities = new();

        /// <summary>
        /// The factory registry used to create entities on demand.
        /// </summary>
        private readonly IMultiFactory<TKey, T, TArgs> _factory;
        private readonly TArgs _args;
        private readonly ExpandMode _expandMode;
        private readonly Dictionary<TKey, int> _rentedCounts = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiPool{TKey,T,TArgs}"/> class.
        /// </summary>
        /// <param name="factory">The factory registry used to create entities for each key.</param>
        /// <param name="args">The arguments passed to the factory when creating entities.</param>
        /// <param name="expandMode">Determines how the pool expands when empty. Defaults to <see cref="ExpandMode.ExpandByOne"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is null.</exception>
        public MultiPool(IMultiFactory<TKey, T, TArgs> factory, TArgs args, ExpandMode expandMode = ExpandMode.ExpandByOne)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _args = args ?? throw new ArgumentNullException(nameof(args));
            _expandMode = expandMode;
        }

        /// <inheritdoc />
        public void Init(TKey key, int count)
        {
            if (!_pooledEntities.TryGetValue(key, out Stack<T> pool))
            {
                pool = new Stack<T>();
                _pooledEntities.Add(key, pool);
            }

            for (int i = 0; i < count; i++)
            {
                T entity = _factory.Create(key, _args);
                this.OnCreate(entity);
                pool.Push(entity);
            }
        }

        /// <inheritdoc />
        public T Rent(TKey key)
        {
            if (!_pooledEntities.TryGetValue(key, out Stack<T> pool))
            {
                pool = new Stack<T>();
                _pooledEntities.Add(key, pool);
            }

            if (!pool.TryPop(out T entity))
                entity = this.Expand(key, pool);

            _rentEntities.Add(entity, key);

            if (!_rentedCounts.TryAdd(key, 1))
                _rentedCounts[key]++;

            this.OnRent(entity);
            return entity;
        }

        private T Expand(TKey key, Stack<T> pool)
        {
            switch (_expandMode)
            {
                case ExpandMode.NoExpand:
                    throw new InvalidOperationException(
                        $"[MultiEntityPool] Pool for key '{key}' is empty and ExpandMode is NoExpand. " +
                        $"Pre-instantiate more entities via Init() or switch to ExpandByOne/ExpandByDoubling.");

                case ExpandMode.ExpandByDoubling:
                    int count = _rentedCounts.TryGetValue(key, out int rented) && rented > 0 ? rented : 1;
                    this.CreateEntities(key, pool, count);
                    pool.TryPop(out T doubled);
                    return doubled;

                case ExpandMode.ExpandByOne:
                default:
                    T entity = _factory.Create(key, _args);
                    this.OnCreate(entity);
                    return entity;
            }
        }

        private void CreateEntities(TKey key, Stack<T> pool, int count)
        {
            for (int i = 0; i < count; i++)
            {
                T entity = _factory.Create(key, _args);
                this.OnCreate(entity);
                pool.Push(entity);
            }
        }

        /// <inheritdoc />
        public void Return(T entity)
        {
            if (!_rentEntities.Remove(entity, out TKey key))
                return;

            if (!_pooledEntities.TryGetValue(key, out Stack<T> pool))
            {
                pool = new Stack<T>();
                _pooledEntities.Add(key, pool);
            }

            if (pool.Contains(entity))
                return;

            if (_rentedCounts.TryGetValue(key, out int count) && count > 0)
                _rentedCounts[key] = count - 1;

            this.OnReturn(entity);
            pool.Push(entity);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (KeyValuePair<TKey, Stack<T>> pool in _pooledEntities)
            foreach (T entity in pool.Value)
                this.OnDispose(entity);

            foreach (T entity in _rentEntities.Keys)
                this.OnDispose(entity);

            _pooledEntities.Clear();
            _rentEntities.Clear();
            _rentedCounts.Clear();
        }

        /// <summary>
        /// Called when a new entity is created for the pool.
        /// </summary>
        /// <param name="entity">The newly created entity.</param>
        protected virtual void OnCreate(T entity)
        {
        }

        /// <summary>
        /// Called when an entity is permanently removed from the pool (e.g., during <see cref="Clear"/>).
        /// </summary>
        /// <param name="entity">The entity being disposed.</param>
        protected virtual void OnDispose(T entity)
        {
        }

        /// <summary>
        /// Called when an entity is rented from a pool.
        /// </summary>
        /// <param name="entity">The rented entity.</param>
        protected virtual void OnRent(T entity)
        {
        }

        /// <summary>
        /// Called when an entity is returned to its pool.
        /// </summary>
        /// <param name="entity">The returned entity.</param>
        protected virtual void OnReturn(T entity)
        {
        }
    }
}