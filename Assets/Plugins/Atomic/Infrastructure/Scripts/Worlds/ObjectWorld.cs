using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace Atomic.Infrastructure
{
    public abstract class ObjectWorld<T> : ObjectCollection<T>, IObjectWorld<T>
    {
        public event Action OnEnabled;

        public event Action OnDisabled;

        public event Action<float> OnTicked;

        public event Action<float> OnFixedTicked;

        public event Action<float> OnLateTicked;

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string Name
        {
            get => _name;
            set => _name = value;
        }

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public bool Enabled => _enabled;

        private string _name;
        private bool _enabled;

        protected ObjectWorld() => _name = string.Empty;

        protected ObjectWorld(params T[] entities)
        {
            _name = string.Empty;
            this.AddRange(entities);
        }

        protected ObjectWorld(string name = null, params T[] entities)
        {
            _name = name;
            this.AddRange(entities);
        }

        protected ObjectWorld(string name, IEnumerable<T> entities)
        {
            _name = name;
            this.AddRange(entities);
        }

        /// <inheritdoc/>
        public void Enable()
        {
            if (_enabled)
                return;

            for (int i = 0; i < _count; i++)
                this.Enable(_slots[_order[i]].value);

            _enabled = true;

            this.NotifyAboutStateChanged();
            this.OnEnabled?.Invoke();
        }

        protected abstract void Enable(T obj);

        /// <inheritdoc/>
        public void Disable()
        {
            if (!_enabled)
                return;

            for (int i = 0; i < _count; i++)
                this.Disable(_slots[_order[i]].value);

            _enabled = false;

            this.NotifyAboutStateChanged();
            this.OnDisabled?.Invoke();
        }

        protected abstract void Disable(T obj);


        /// <inheritdoc/>
        public void Tick(float deltaTime)
        {
            if (!_enabled)
                return;

            for (int i = 0; i < _count; i++)
                this.Tick(_slots[_order[i]].value, deltaTime);

            this.OnTicked?.Invoke(deltaTime);
        }

        protected abstract void Tick(T obj, float deltaTime);

        /// <inheritdoc/>
        public void FixedTick(float deltaTime)
        {
            if (!_enabled)
                return;

            for (int i = 0; i < _count; i++)
                FixedTick(_slots[_order[i]].value, deltaTime);

            this.OnFixedTicked?.Invoke(deltaTime);
        }

        protected abstract void FixedTick(T obj, float deltaTime);

        /// <inheritdoc/>
        public void LateTick(float deltaTime)
        {
            if (!_enabled)
                return;

            for (int i = 0; i < _count; i++)
                this.LateTick(_slots[_order[i]].value, deltaTime);

            this.OnLateTicked?.Invoke(deltaTime);
        }

        protected abstract void LateTick(T obj, float deltaTime);


        /// <summary>
        /// Enables added entity if world is enabled.
        /// </summary>
        /// <param name="entity">Added entity</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnAdd(T entity)
        {
            if (_enabled) this.Enable(entity);
        }

        /// <summary>
        /// Disables removed entity if world is enabled. 
        /// </summary>
        /// <param name="entity">Removed entity</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnRemove(T entity)
        {
            if (_enabled) this.Disable(entity);
        }

        /// <summary>
        /// Disables this state, clear all entities and release all events
        /// </summary>
        public override void Dispose()
        {
            if (_enabled)
                this.Disable();

            base.Dispose();

            //Unsubscribe events:
            this.OnEnabled = null;
            this.OnDisabled = null;
            this.OnTicked = null;
            this.OnFixedTicked = null;
            this.OnLateTicked = null;
        }
    }
}