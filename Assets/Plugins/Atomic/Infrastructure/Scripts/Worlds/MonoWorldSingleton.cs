using System;
using UnityEngine;

namespace Atomic.Infrastructure
{
    public abstract class MonoWorldSingleton<E> : MonoWorld<E>
    {
        [Tooltip("If enabled, this object will persist across scene loads.")]
        [SerializeField]
        private bool dontDestroyOnLoad;

        private static MonoWorldSingleton<E> _instance;

        /// <summary>
        /// Gets the singleton instance.
        /// Throws an exception if not found.
        /// </summary>
        public static MonoWorldSingleton<E> Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindAnyObjectByType<MonoWorldSingleton<E>>(FindObjectsInactive.Exclude);

                if (_instance == null)
                    throw new Exception(
                        $"MonoEntityWorldSingleton<{typeof(E).Name}> was not found in the scene.");

                return _instance;
            }
        }

        /// <summary>
        /// Attempts to get the singleton instance.
        /// </summary>
        public static bool TryGetInstance(out MonoWorldSingleton<E> instance)
        {
            if (_instance != null)
            {
                instance = _instance;
                return true;
            }

            instance = _instance = FindAnyObjectByType<MonoWorldSingleton<E>>(FindObjectsInactive.Exclude);
            return instance != null;
        }

        /// <summary>
        /// Initializes singleton instance.
        /// </summary>
        protected override void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError(
                    $"Duplicate MonoEntityWorldSingleton<{typeof(E).Name}> detected on '{name}'. Destroying this instance.",
                    this);

                Destroy(gameObject);
                return;
            }

            _instance = this;

            base.Awake();

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Clears singleton reference when destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_instance == this)
                _instance = null;
        }
    }
}