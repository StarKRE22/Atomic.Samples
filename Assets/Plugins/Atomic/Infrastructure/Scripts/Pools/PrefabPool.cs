#if UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class PrefabPool<T, U> : MonoBehaviour, IPrefabPool<T, U> where U : Component, T
    {
        private const string NUMBER_PATTERN = @"\s*\(\d+\)$";

        internal struct Pool
        {
            public Stack<U> stack;
            public Transform container;
            public GameObject go;
        }

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private readonly Dictionary<string, Pool> _pools = new();
        private readonly Dictionary<string, int> _rentedCounts = new();

        /// <summary>
        /// If not assigned, defaults to the GameObject this script is attached to.
        /// </summary>
        [Space]
        [Tooltip("Root container for pooled entities")]
        [SerializeField]
        private Transform _container;

        [Tooltip("Should don't destroy if scene changed?")]
        [SerializeField]
        private bool _dontDestroyOnLoad;

        [Space]
        [Tooltip("Determines how the pool expands when empty.\n" +
                 "ExpandByOne: Creates one new entity per request.\n" +
                 "ExpandByDoubling: Doubles the current pooled count (e.g. 10 → 20).\n" +
                 "NoExpand: Throws an exception when the pool is empty.")]
        [SerializeField]
        private ExpandMode _expandMode = ExpandMode.ExpandByOne;

        protected virtual void Awake()
        {
            if (_container == null)
                _container = this.transform;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Pre-initializes the pool for a specific prefab with a defined number of inactive entities.
        /// </summary>
        /// <param name="prefab">The prefab to pool.</param>
        /// <param name="count">How many instances to pre-instantiate.</param>
        public void Init(U prefab, int count)
        {
            string name = this.GetObjectName(prefab);
            if (!_pools.TryGetValue(name, out Pool pool))
            {
                pool = this.CreatePool(name);
                _pools.Add(name, pool);
            }

            for (int i = 0; i < count; i++)
            {
                U entity = CreateEntity(prefab, pool.container);
                entity.name = name;
                pool.stack.Push(entity);
            }
        }

        public async ValueTask InitAsync(U prefab, int initialCount)
        {
            string name = this.GetObjectName(prefab);
            if (!_pools.TryGetValue(name, out Pool pool))
            {
                pool = this.CreatePool(name);
                _pools.Add(name, pool);
            }

            U[] entities = await this.CreateAsync(prefab, initialCount, _container);
            foreach (U entity in entities)
            {
                this.OnCreate(entity);
                entity.name = name;
                pool.stack.Push(entity);
            }
        }

        public T Rent(U prefab) => this.Rent(prefab, Vector3.zero, Quaternion.identity);

        public T Rent(U prefab, Transform parent) => this.Rent(prefab, parent.position, parent.rotation);

        public T Rent(U prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            string name = GetObjectName(prefab);

            if (!_pools.TryGetValue(name, out Pool pool))
            {
                pool = this.CreatePool(name);
                _pools.Add(name, pool);
            }

            if (pool.stack.TryPop(out U entity))
            {
                Transform tf = entity.transform;
                tf.SetParent(parent, false);
                tf.SetPositionAndRotation(position, rotation);
            }
            else
            {
                entity = this.Expand(prefab, pool);
                entity.name = name;
                entity.transform.SetPositionAndRotation(position, rotation);
            }

            this.OnRent(entity);

            if (!_rentedCounts.TryAdd(name, 1))
                _rentedCounts[name]++;

            return entity;
        }

        private U Expand(U prefab, Pool pool)
        {
            string name = GetObjectName(prefab);
            switch (_expandMode)
            {
                case ExpandMode.NoExpand:
                    throw new InvalidOperationException(
                        $"[PrefabPool] Pool '{name}' for prefab '{prefab.name}' is empty " +
                        "and ExpandMode is NoExpand. Pre-instantiate more entities via Init() " +
                        "or switch to ExpandByOne/ExpandByDoubling.");

                case ExpandMode.ExpandByDoubling:
                    int count = _rentedCounts.TryGetValue(name, out int rented) && rented > 0 ? rented : 1;
                    this.CreateEntities(prefab, pool, count);
                    pool.stack.TryPop(out U doubled);
                    return doubled;

                case ExpandMode.ExpandByOne:
                default:
                    return this.CreateEntity(prefab, pool.container);
            }
        }

        private void CreateEntities(U prefab, Pool pool, int count)
        {
            for (int i = 0; i < count; i++)
            {
                U entity = this.CreateEntity(prefab, pool.container);
                entity.name = GetObjectName(prefab);
                pool.stack.Push(entity);
            }
        }

        /// <inheritdoc />
        public void Return(T entity)
        {
            U sceneEntity = this.Cast(entity);
            string name = GetObjectName(sceneEntity);

            if (!_pools.TryGetValue(name, out Pool pool))
            {
                pool = this.CreatePool(name);
                _pools.Add(name, pool);
            }

            if (pool.stack.Contains(sceneEntity))
                return;

            this.OnReturn(sceneEntity);

            sceneEntity.transform.SetParent(pool.container, false);
            pool.stack.Push(sceneEntity);

            if (_rentedCounts.TryGetValue(name, out int rented) && rented > 0)
                _rentedCounts[name] = rented - 1;
        }

        /// <summary>
        /// Clears the pool for a specific prefab and destroys all associated entities and container.
        /// </summary>
        /// <param name="prefab">The prefab whose pool should be cleared.</param>
        public void Dispose(U prefab)
        {
            string objName = this.GetObjectName(prefab);
            if (!_pools.Remove(objName, out Pool pool))
                return;

            foreach (U entity in pool.stack)
            {
                this.OnDispose(entity);
                this.Destroy(entity);
            }

            _rentedCounts.Remove(objName);
            Destroy(pool.go);
        }

        /// <summary>
        /// Clears all pools and destroys all pooled entities.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, Pool> pair in _pools)
            {
                Pool pool = pair.Value;
                foreach (U entity in pool.stack)
                {
                    this.OnDispose(entity);
                    this.Destroy(entity);
                }

                Destroy(pool.go);
            }

            _pools.Clear();
            _rentedCounts.Clear();
        }

        /// <summary>
        /// Called when a new entity instance is created for pooling.
        /// Use this to apply default inactive state or setup.
        /// </summary>
        /// <param name="entity">The new pooled entity.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnCreate(U entity) => entity.gameObject.SetActive(false);

        /// <summary>
        /// Called when an entity is rented from the pool.
        /// Default behavior activates the entity.
        /// </summary>
        /// <param name="entity">The rented entity.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnRent(U entity) => entity.gameObject.SetActive(true);

        /// <summary>
        /// Called when an entity is returned to the pool.
        /// Default behavior deactivates the entity.
        /// </summary>
        /// <param name="entity">The returned entity.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnReturn(U entity) => entity.gameObject.SetActive(false);

        /// <summary>
        /// Called when a pooled entity is destroyed (e.g., during pool cleanup).
        /// Override to dispose resources or unregister events.
        /// </summary>
        /// <param name="entity">The entity being disposed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnDispose(U entity)
        {
        }

        /// <summary>
        /// Extracts a clean name from a prefab or entity instance, stripping Unity-generated suffixes like " (1)".
        /// Used internally to group entities by prefab type.
        /// </summary>
        /// <param name="entity">The entity to extract a base name from.</param>
        /// <returns>A clean prefab name for use as a pool key.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual string GetObjectName(U entity)
        {
            return Regex.Replace(entity.name, NUMBER_PATTERN, string.Empty).Trim();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Pool CreatePool(string name)
        {
            Stack<U> stack = new Stack<U>();
            Transform container = new GameObject($"<{name}s>").transform;
            container.parent = _container;

            return new Pool
            {
                stack = stack,
                container = container
            };
        }

        /// <summary>
        /// Instantiates a new entity instance from the prefab and initializes it.
        /// </summary>
        /// <returns>The newly created entity.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private U CreateEntity(U prefab, Transform container)
        {
            U entity = this.Create(prefab, container);
            this.OnCreate(entity);
            return entity;
        }

        protected virtual U Create(U prefab, Transform container) => Instantiate(prefab, container);

        protected virtual async ValueTask<U[]> CreateAsync(U prefab, int count, Transform container) =>
            await InstantiateAsync(prefab, count, container);

        protected virtual U Cast(T entity) => (U) entity;

        protected virtual void Destroy(U entity) => GameObject.Destroy(entity.gameObject);
    }
}
#endif