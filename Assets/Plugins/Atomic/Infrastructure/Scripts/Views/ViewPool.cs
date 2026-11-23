#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class ViewPool<TKey, TModel, TView> : MonoBehaviour
        where TModel : class
        where TView : View<TModel>
    {
        private static readonly ProfilerMarker s_rentMarker = new($"ViewPool<{typeof(TModel).Name}>.Rent");
        private static readonly ProfilerMarker s_returnMarker = new($"ViewPool<{typeof(TModel).Name}>.Return");

        [Tooltip("The parent transform under which all pooled views will be stored")]
        [SerializeField]
        private Transform container;

        [SerializeField]
        private int initialCapacity = 32;

        [Space]
        [Tooltip("A list of view catalogs to preload view prefabs from on Awake")]
        [SerializeField]
        private ViewCatalog<TModel, TView>[] catalogs;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private readonly Dictionary<TKey, TView> _prefabs = new();

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private readonly Dictionary<TKey, Stack<TView>> _pools = new();

        private protected virtual void Awake()
        {
            this.RegisterCatalogs();
        }

        /// <summary>
        /// Registers a new view prefab to the pool by name.
        /// </summary>
        /// <param name="key">The name identifier for the view prefab.</param>
        /// <param name="prefab">The prefab to register.</param>
        public void Register(TKey key, TView prefab) => _prefabs.Add(key, prefab);

        /// <summary>
        /// Removes a registered prefab from the pool.
        /// </summary>
        /// <param name="key">The name of the prefab to remove.</param>
        public void Unregister(TKey key) => _prefabs.Remove(key);

        /// <summary>
        /// Add all prefabs from a given catalogue to the internal registry.
        /// </summary>
        /// <param name="catalog">The catalogue containing view prefabs to register.</param>
        public void Register(ViewCatalog<TModel, TView> catalog)
        {
            for (int i = 0, count = catalog.Count; i < count; i++)
            {
                TView value = catalog.GetPrefab(i);
                _prefabs.Add(this.GetKey(value), value);
            }
        }

        /// <summary>
        /// Removes all prefabs from a given catalogue from the internal registry.
        /// </summary>
        /// <param name="catalog">The catalogue containing view prefabs to unregister.</param>
        public void Unregister(ViewCatalog<TModel, TView> catalog)
        {
            for (int i = 0, count = catalog.Count; i < count; i++)
            {
                TView prefab = catalog.GetPrefab(i);
                _prefabs.Remove(this.GetKey(prefab));
            }
        }

        /// <summary>
        /// Clears all pooled instances and destroys their GameObjects.
        /// </summary>
        public void Clear()
        {
            foreach (Stack<TView> pool in _pools.Values)
            {
                foreach (TView view in pool)
                    Destroy(view.gameObject);

                pool.Clear();
            }

            _pools.Clear();
        }

        public async ValueTask InitAsync(TKey key, int count)
        {
            if (!_prefabs.TryGetValue(key, out TView prefab))
                throw new KeyNotFoundException(
                    $"EntityView<{typeof(TModel).Name}> with \"{key}\" was not present in pool!");

            AsyncInstantiateOperation<TView> operation = InstantiateAsync(prefab, count, this.container);
            TView[] views = await operation;

            int viewCount = views.Length;
            Stack<TView> pool = this.GetOrCreatePool(key, Mathf.Max(viewCount, this.initialCapacity));

            for (int i = 0; i < viewCount; i++)
            {
                TView view = views[i];
                view.gameObject.SetActive(false);
                pool.Push(view);
            }
        }

        public void Init(TKey key, int count)
        {
            if (!_prefabs.TryGetValue(key, out TView prefab))
                throw new KeyNotFoundException(
                    $"EntityView<{typeof(TModel).Name}> with name \"{key}\" was not present in pool!");

            Stack<TView> pool = this.GetOrCreatePool(key, Mathf.Max(count, this.initialCapacity));
            for (int i = 0; i < count; i++)
            {
                TView view = Instantiate(prefab, this.container);
                view.gameObject.SetActive(false);
                pool.Push(view);
            }
        }

        private void RegisterCatalogs()
        {
            if (this.catalogs != null)
                for (int i = 0, count = this.catalogs.Length; i < count; i++)
                    this.Register(this.catalogs[i]);
        }

        internal TView Rent(TKey key, Transform parent)
        {
            using (s_rentMarker.Auto())
            {
                Stack<TView> pool = this.GetOrCreatePool(key, this.initialCapacity);
                if (pool.TryPop(out TView view))
                {
                    view.transform.SetParent(parent);
                    view.gameObject.SetActive(true);
                    return view;
                }

                return !_prefabs.TryGetValue(key, out TView prefab)
                    ? throw new KeyNotFoundException(
                        $"EntityView<{typeof(TModel).Name}> with key \"{key}\" was not present in pool!")
                    : Instantiate(prefab, parent);
            }
        }

        internal void Return(TView view)
        {
            using (s_returnMarker.Auto())
            {
                if (!view)
                {
                    Debug.LogWarning($"ViewPool<{typeof(TModel).Name}>: Attempt to return a null or destroyed view!");
                    return;
                }

                Stack<TView> pool = this.GetOrCreatePool(
                    this.GetKey(view),
                    this.initialCapacity
                );

                if (pool.Contains(view))
                {
                    Debug.LogWarning(
                        $"ViewPool<{typeof(TModel).Name}>: View '{view.name}' has already been returned to the pool!",
                        view
                    );
                    return;
                }

                view.transform.SetParent(this.container);
                view.gameObject.SetActive(false);

                pool.Push(view);
            }
        }

        protected abstract TKey GetKey(TView view);

        private Stack<TView> GetOrCreatePool(TKey key, int initialCapacity)
        {
            if (!_pools.TryGetValue(key, out Stack<TView> pool))
            {
                pool = new Stack<TView>(initialCapacity);
                _pools.Add(key, pool);
            }

            return pool;
        }
    }
}
#endif