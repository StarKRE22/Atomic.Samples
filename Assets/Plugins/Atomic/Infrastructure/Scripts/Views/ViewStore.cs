#if UNITY_5_3_OR_NEWER
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class ViewStore<TKey, TModel, TView> : MonoBehaviour,
        IReadOnlyCollection<KeyValuePair<TModel, TView>>
        where TModel : class
        where TView : View<TModel>
    {
        private static readonly ProfilerMarker s_removeMarker = new($"ViewCollection<{typeof(TModel).Name}>.Remove");
        private static readonly ProfilerMarker s_addMarker = new($"ViewCollection<{typeof(TModel).Name}>.Add");

        private static readonly ArrayPool<TModel> s_arrayPool = ArrayPool<TModel>.Shared;

        /// <summary>
        /// Raised when a view is spawned for a newly added entity.
        /// </summary>
        public event Action<TModel, TView> OnAdded;

        /// <summary>
        /// Raised when a view is removed for a despawned or removed entity.
        /// </summary>
        public event Action<TModel, TView> OnRemoved;

        /// <summary>
        /// Gets the number of active entity views currently tracked by this collection.
        /// </summary>
        public int Count => _views.Count;

        internal ViewPool<TKey, TModel, TView> Pool => _pool;

        [Tooltip("The viewport or container under which views will be placed in the scene hierarchy")]
        [SerializeField]
        private Transform viewport;

        [Space, SerializeField]
        private ViewPool<TKey, TModel, TView> _pool;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private readonly Dictionary<TModel, TView> _views = new();

        /// <summary>
        /// Gets the view instance associated with the specified entity.
        /// </summary>
        /// <param name="entity">The entity whose view is requested.</param>
        /// <returns>The active <see cref="EntityView{E}"/> instance associated with the entity.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the entity is not in the collection.</exception>
        public TView Get(TModel entity) => _views[entity];

        /// <summary>
        /// Tries to retrieve the view for a given entity.
        /// </summary>
        /// <param name="entity">The entity whose view is requested.</param>
        /// <param name="view">The active <see cref="EntityView{E}"/> instance associated with the entity.</param>
        /// <returns>"true" if a view exists, "false" otherwise.</returns>
        public bool TryGet(TModel entity, out TView view) => _views.TryGetValue(entity, out view);

        /// <summary>
        /// Checks whether a view exists for the specified entity.
        /// </summary>
        /// <param name="entity">The entity whose view is requested.</param>
        /// <returns>"true" if a view exists, "false" otherwise.</returns>
        public bool Contains(TModel entity) => _views.ContainsKey(entity);

        /// <summary>
        /// Creates and shows a view for the specified entity, if it does not already exist.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        public TView Add(TModel entity)
        {
            using (s_addMarker.Auto())
            {
                if (_views.TryGetValue(entity, out TView view))
                    throw new Exception($"ViewCollection<{typeof(TModel).Name}>: {entity} is already exists!");

                TKey key = this.GetKey(entity);
                view = _pool.Rent(key, this.viewport);
                view.Show(entity);

                _views.Add(entity, view);
                this.OnAdded?.Invoke(entity, view);
                return view;
            }
        }

        public void GetOrAdd(TModel entity, out TView view)
        {
            using (s_addMarker.Auto())
            {
                if (_views.TryGetValue(entity, out view))
                    return;

                TKey key = this.GetKey(entity);
                view = _pool.Rent(key, this.viewport);
                view.Show(entity);

                _views.Add(entity, view);
                this.OnAdded?.Invoke(entity, view);
            }
        }

        protected abstract TKey GetKey(TModel entity);

        /// <summary>
        /// Hides and returns the view associated with the specified entity to the view pool.
        /// </summary>
        /// <param name="entity">The entity whose view should be removed.</param>
        public bool Remove(TModel entity)
        {
            using (s_removeMarker.Auto())
            {
                if (!_views.Remove(entity, out TView view))
                    return false;

                this.OnRemoved?.Invoke(entity, view);
                view.Hide();
                _pool.Return(view);
                return true;
            }
        }

        public bool Remove(TView view) => this.Remove(view.Model);

        public Dictionary<TModel, TView>.Enumerator GetEnumerator() => _views.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection of entity-view pairs.
        /// </summary>
        /// <returns>An enumerator of <see cref="KeyValuePair{TKey, TValue}"/> containing entity-view pairs.</returns>
        IEnumerator<KeyValuePair<TModel, TView>> IEnumerable<KeyValuePair<TModel, TView>>.GetEnumerator() =>
            _views.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection of entity-view pairs.
        /// </summary>
        /// <returns>An enumerator containing entity-view pairs.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _views.GetEnumerator();

        /// <summary>
        /// Removes active entity views, returning them to the view pool.
        /// </summary>
        public void Clear()
        {
            int viewCount = _views.Count;
            if (viewCount == 0)
                return;

            TModel[] buffer = s_arrayPool.Rent(viewCount);
            _views.Keys.CopyTo(buffer, 0);

            try
            {
                for (int i = 0; i < viewCount; i++)
                    this.Remove(buffer[i]);
            }
            finally
            {
                s_arrayPool.Return(buffer);
            }
        }
    }
}
#endif