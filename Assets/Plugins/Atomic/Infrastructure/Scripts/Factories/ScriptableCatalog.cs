#if UNITY_5_3_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class ScriptableCatalog<TKey, T, TFactory, TArgs> : ScriptableObject,
        IMultiFactory<TKey, T, TArgs>,
        IReadOnlyDictionary<TKey, TFactory>
        where TFactory : ScriptableObject, IFactory<T, TArgs>
        where TArgs : IArgs
    {
#if ODIN_INSPECTOR
        [AssetsOnly]
        [ValidateInput(nameof(ValidateFactories), "Factories contain duplicates or nulls")]
        [ListDrawerSettings(ShowFoldout = true)]
#endif
        [SerializeField]
        private TFactory[] _factories;

        private Dictionary<TKey, TFactory> _factoryMap;

        #region Factory API

        /// <inheritdoc />
        public T Create(TKey key, TArgs args)
        {
            EnsureInitialized();

            return !_factoryMap.TryGetValue(key, out var factory)
                ? throw new KeyNotFoundException($"Factory with key '{key}' not found in '{name}'")
                : factory.Create(args);
        }

        /// <inheritdoc />
        public bool TryCreate(TKey key, TArgs args, out T entity)
        {
            EnsureInitialized();

            if (_factoryMap.TryGetValue(key, out var factory))
            {
                entity = factory.Create(args);
                return true;
            }

            entity = default;
            return false;
        }

        /// <inheritdoc />
        public bool Contains(TKey key)
        {
            EnsureInitialized();
            return _factoryMap.ContainsKey(key);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Extracts a key from a given factory.
        /// </summary>
        protected abstract TKey GetKey(TFactory factory);

        private void EnsureInitialized()
        {
            if (_factoryMap != null)
                return;

            int capacity = _factories?.Length ?? 0;
            _factoryMap = new Dictionary<TKey, TFactory>(capacity);

            if (_factories == null || _factories.Length == 0)
            {
                Debug.LogWarning($"{name} has no factories assigned", this);
                return;
            }

            foreach (var factory in _factories)
            {
                if (factory == null)
                    continue;

                var key = GetKey(factory);
                if (_factoryMap.ContainsKey(key))
                    Debug.LogWarning($"Duplicate key '{key}' in {name}. Overwriting with '{factory.name}'", this);

                _factoryMap[key] = factory;
            }
        }

#if ODIN_INSPECTOR
        private bool ValidateFactories(TFactory[] factories)
        {
            if (factories == null)
                return true;

            var set = new HashSet<TKey>();
            foreach (TFactory factory in factories)
            {
                if (factory == null)
                    return false;

                var key = GetKey(factory);
                if (!set.Add(key))
                    return false;
            }

            return true;
        }
#endif

        #endregion

        #region IReadOnlyDictionary

        public int Count
        {
            get
            {
                EnsureInitialized();
                return _factoryMap.Count;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                EnsureInitialized();
                return _factoryMap.Keys;
            }
        }

        public IEnumerable<TFactory> Values
        {
            get
            {
                EnsureInitialized();
                return _factoryMap.Values;
            }
        }

        public TFactory this[TKey key]
        {
            get
            {
                EnsureInitialized();
                return _factoryMap[key];
            }
        }

        public bool ContainsKey(TKey key)
        {
            EnsureInitialized();
            return _factoryMap.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TFactory value)
        {
            EnsureInitialized();
            return _factoryMap.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TFactory>> GetEnumerator()
        {
            EnsureInitialized();
            return _factoryMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
#endif