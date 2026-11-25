#if UNITY_5_3_OR_NEWER
using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// A singleton-based implementation of <see cref="EntityViewManager{K,E,V}"/>.
    /// Ensures that only one instance exists in the scene (or globally if marked as persistent).
    /// </summary>
    /// <typeparam name="E">Entity type.</typeparam>
    /// <typeparam name="V">View type.</typeparam>
    public class EntityViewManagerSingleton : EntityViewManager<string, IEntity, EntityView>
    {
#if ODIN_INSPECTOR
        [PropertyOrder(-10)]
        [DisableInPlayMode]
        [PropertySpace(SpaceBefore = 0)]
#endif
        [Tooltip("If enabled, this object will not be destroyed when loading a new scene.")]
        [SerializeField]
        private bool dontDestroyOnLoad;

        private static EntityViewManagerSingleton _instance;

        /// <summary>
        /// Gets the singleton instance.
        /// Throws an exception if no instance is found in the scene.
        /// </summary>
        public static EntityViewManagerSingleton Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindAnyObjectByType<EntityViewManagerSingleton>(FindObjectsInactive.Exclude);

                if (_instance == null)
                    throw new Exception(
                        $"Singleton of type {nameof(EntityViewManagerSingleton)} was not found in the scene.");

                return _instance;
            }
        }

        /// <summary>
        /// Attempts to get the singleton instance without throwing.
        /// </summary>
        public static bool TryGetInstance(out EntityViewManagerSingleton instance)
        {
            if (_instance != null)
            {
                instance = _instance;
                return true;
            }

            instance = _instance = FindAnyObjectByType<EntityViewManagerSingleton>(FindObjectsInactive.Exclude);
            return instance != null;
        }

        /// <summary>
        /// Unity Awake lifecycle method.
        /// Initializes the singleton instance.
        /// </summary>
        private void Awake()
        {
            this.InitializeSingleton();
        }

        /// <summary>
        /// Ensures only one instance exists and optionally makes it persistent.
        /// </summary>
        private void InitializeSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError(
                    $"Duplicate singleton of type {nameof(EntityViewManagerSingleton)} detected on '{name}'. Destroying this instance.",
                    this);

                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Clears the singleton reference when the object is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        protected override string GetKey(IEntity entity) => entity.Name;
    }
}
#endif