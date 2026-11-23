using System;
using System.Runtime.CompilerServices;

using Unity.Profiling;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class SystemBase<T> : ISystem
    {
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private bool _enabled;

#if ENABLE_PROFILER
        private ProfilerMarker _marker;
#endif

        /// <summary>
        /// Read-only collection of entities processed by this system.
        /// </summary>
        private protected readonly IReadOnlyStore<T> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySystemBase{E}"/> class.
        /// </summary>
        /// <param name="store">Source collection of entities.</param>
        /// <param name="settings">System configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="store"/> or <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        protected SystemBase(IReadOnlyStore<T> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));

#if ENABLE_PROFILER
            _marker = new ProfilerMarker(this.GetType().Name + ".Update");
#endif
        }

        /// <summary>
        /// Enables the system, subscribes to entity collection events,
        /// and registers all existing entities.
        /// </summary>
        public void Enable()
        {
            if (_enabled)
                return;

            _enabled = true;

            foreach (T entity in _store)
                this.OnAdd(entity);

            _store.OnAdded += this.OnAdd;
            _store.OnRemoved += this.OnRemove;

            this.OnEnable();
        }

        /// <summary>
        /// Called after the system has been enabled.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Disables the system, unsubscribes from entity collection events,
        /// and unregisters all tracked entities.
        /// </summary>
        public void Disable()
        {
            if (!_enabled)
                return;

            _enabled = false;

            this.OnDisable();

            _store.OnAdded -= this.OnAdd;
            _store.OnRemoved -= this.OnRemove;

            foreach (T entity in _store)
                this.OnRemove(entity);
        }

        /// <summary>
        /// Called before the system is disabled.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// Updates the system.
        /// The execution time is measured and the processing batch size
        /// is automatically adjusted to fit within the configured frame budget.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the previous update.</param>
        public void Update(float deltaTime)
        {
            if (!_enabled)
                return;
            
#if ENABLE_PROFILER
            using (_marker.Auto())
#endif
                this.OnUpdate(deltaTime);
        }

        /// <summary>
        /// Performs the system update.
        /// </summary>
        /// <param name="batchSize">Maximum number of entities to process during this update.</param>
        /// <param name="deltaTime">Time elapsed since the previous update.</param>
        protected abstract void OnUpdate(float deltaTime);

        /// <summary>
        /// Called when an entity is added to the source collection
        /// or when the system is enabled.
        /// </summary>
        /// <param name="entity">The added entity.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnAdd(T entity)
        {
        }

        /// <summary>
        /// Called when an entity is removed from the source collection
        /// or when the system is disabled.
        /// </summary>
        /// <param name="entity">The removed entity.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnRemove(T entity)
        {
        }

        /// <summary>
        /// Releases all resources used by the system.
        /// </summary>
        public abstract void Dispose();
    }
}