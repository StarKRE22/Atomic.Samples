using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class MonoObjectWorld<T> : MonoBehaviour, IObjectWorld<T>
    {
#if ODIN_INSPECTOR
        [HideInPlayMode]
#endif
        [Header("Lifecycle")]
        [SerializeField]
        [Tooltip("Enable automatic syncing with Unity MonoBehaviour lifecycle (Start/OnEnable/OnDisable).")]
        protected bool useUnityLifecycle = true;

        [Tooltip("Should don't destroy if scene changed?")]
        [SerializeField]
        private bool dontDestroyOnLoad;

#if ODIN_INSPECTOR
        [HideInPlayMode]
        [GUIColor(0f, 0.83f, 1f)]
#endif
        [Header("Collecting")]
        [Tooltip("If this option is enabled then EntityWorld add all Entities on a scene on Awake()")]
        [SerializeField]
        protected bool collectOnAwake = true;

#if ODIN_INSPECTOR
        [ShowIf(nameof(collectOnAwake))]
#endif
        [Tooltip("If this option is enabled then EntityWorld scan inactive Entities on a scene also")]
        [SerializeField]
        private bool includeInactiveOnCollect = true;

        protected virtual void Awake()
        {
            if (this.collectOnAwake)
                this.CollectObjectsOnAwake();

            if (this.dontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
        }

        private void CollectObjectsOnAwake()
        {
            using PooledObject<List<GameObject>> roots = ListPool<GameObject>.Get(out List<GameObject> rootObjects);
            using PooledObject<List<T>> children = ListPool<T>.Get(out List<T> entities);

            this.gameObject.scene.GetRootGameObjects(rootObjects);

            foreach (GameObject root in rootObjects)
            {
                entities.Clear();
                root.GetComponentsInChildren(includeInactiveOnCollect, entities);

                foreach (T entity in entities)
                {
                    CollectOnAwake(entity);
                    Add(entity);
                }
            }
        }

        protected virtual void CollectOnAwake(T child)
        {
        }

        protected virtual void OnEnable()
        {
            if (this.useUnityLifecycle && this.isStarted)
                this.Enable();
        }

        protected virtual void Start()
        {
            if (this.useUnityLifecycle)
            {
                this.Enable();
                this.isStarted = true;
            }
        }

        protected virtual void Update()
        {
            if (this.useUnityLifecycle)
                this.Tick(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            if (this.useUnityLifecycle)
                this.FixedTick(Time.fixedDeltaTime);
        }

        protected virtual void LateUpdate()
        {
            if (this.useUnityLifecycle)
                this.LateTick(Time.fixedDeltaTime);
        }

        protected virtual void OnDisable()
        {
            if (this.useUnityLifecycle && this.isStarted)
                this.Disable();
        }

        protected virtual void OnDestroy()
        {
            if (this.useUnityLifecycle)
                this.Dispose();
        }

        protected abstract ObjectWorld<T> Inner { get; }

        private bool isStarted;

        /// <inheritdoc />
        public event Action OnStateChanged
        {
            add => Inner.OnStateChanged += value;
            remove => Inner.OnStateChanged -= value;
        }

        /// <inheritdoc />
        public string Name
        {
            get => this.name;
            set => this.name = value;
        }

        public T this[int index] => Inner[index];

        public bool TryGetAt(int index, out T entity) => Inner.TryGetAt(index, out entity);

        public event Action<T> OnAdded
        {
            add => Inner.OnAdded += value;
            remove => Inner.OnAdded -= value;
        }

        /// <inheritdoc />
        public event Action<T> OnRemoved
        {
            add => Inner.OnRemoved += value;
            remove => Inner.OnRemoved -= value;
        }

        public int Count => Inner.Count;

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Inner.CopyTo(array, arrayIndex);
        }


        
        
        /// <inheritdoc />
        public bool IsReadOnly => Inner.IsReadOnly;

        /// <inheritdoc />
        int IObjectCollection<T>.Count => Inner.Count;

        /// <inheritdoc />
        int ICollection<T>.Count => Inner.Count;

        /// <inheritdoc />
        int IReadOnlyCollection<T>.Count => Inner.Count;

        /// <inheritdoc />
        void ICollection<T>.Add(T item) => Inner.Add(item);

        /// <inheritdoc />
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Inner.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        void IObjectCollection<T>.CopyTo(T[] array, int arrayIndex) => Inner.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        void IReadOnlyObjectCollection<T>.CopyTo(T[] array, int arrayIndex) => Inner.CopyTo(array, arrayIndex);

        /// <inheritdoc cref="NUnit.Framework.Contains" />
        public bool Contains(T entity) => Inner.Contains(entity);

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public bool Add(T entity) => Inner.Add(entity);

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public bool Remove(T entity) => Inner.Remove(entity);

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public void Clear() => Inner.Clear();

        /// <inheritdoc />
        public void CopyTo(ICollection<T> results) => Inner.CopyTo(results);

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => Inner.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => Inner.GetEnumerator();

        public event Action OnEnabled
        {
            add => Inner.OnEnabled += value;
            remove => Inner.OnEnabled -= value;
        }

        /// <inheritdoc />
        public event Action OnDisabled
        {
            add => Inner.OnDisabled += value;
            remove => Inner.OnDisabled -= value;
        }

        /// <inheritdoc />
        public event Action<float> OnTicked
        {
            add => Inner.OnTicked += value;
            remove => Inner.OnTicked -= value;
        }

        /// <inheritdoc />
        public event Action<float> OnFixedTicked
        {
            add => Inner.OnFixedTicked += value;
            remove => Inner.OnFixedTicked -= value;
        }

        /// <inheritdoc />
        public event Action<float> OnLateTicked
        {
            add => Inner.OnLateTicked += value;
            remove => Inner.OnLateTicked -= value;
        }

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Title("Debug")]
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        public bool Enabled => Inner.Enabled;

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public void Enable() => Inner.Enable();

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public void Disable() => Inner.Disable();

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public void Tick(float deltaTime) => Inner.Tick(deltaTime);

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public void FixedTick(float deltaTime) => Inner.FixedTick(deltaTime);

        /// <inheritdoc />
#if ODIN_INSPECTOR
        [Button, HideInEditorMode]
#endif
        public void LateTick(float deltaTime) => Inner.LateTick(deltaTime);

        /// <inheritdoc />
        public void Dispose() => Inner?.Dispose();

        /// <summary>
        /// Creates a new inactive <see cref="GameObject"/> with an attached <see cref="MonoEntityWorld{E}"/> component.
        /// </summary>
        /// <param name="name">The name of the GameObject and world instance.</param>
        /// <param name="scanEntities">If true, the world will scan the scene for entities on Awake.</param>
        /// <param name="entities">Optional entities to add immediately after creation.</param>
        /// <returns>The initialized <see cref="MonoEntityWorld{E}"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Create<R>(
            string name = null,
            bool scanEntities = true,
            bool useUnityLifecycle = true
        ) where R : MonoObjectWorld<R>
        {
            GameObject go = new GameObject();
            go.SetActive(false);

            R world = go.AddComponent<R>();
            world.Name = name;
            world.collectOnAwake = scanEntities;
            world.useUnityLifecycle = useUnityLifecycle;

            go.SetActive(true);
            return world;
        }

        /// <summary>
        /// Destroys the <see cref="MonoEntityWorld{E}"/> and its associated GameObject after an optional delay.
        /// </summary>
        /// <typeparam name="T">The type of scene entity managed by the world.</typeparam>
        /// <param name="world">The world instance to destroy.</param>
        /// <param name="t">Optional delay in seconds before destruction. Default is 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(MonoObjectWorld<T> world, float t = 0)
        {
            if (world)
                GameObject.Destroy(world.gameObject, t);
        }
    }
}