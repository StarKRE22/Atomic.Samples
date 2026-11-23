using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public class Pool<T, TArgs> : IPool<T> where TArgs : IArgs
    {
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private protected readonly Stack<T> _pooledObjects = new();

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private protected readonly HashSet<T> _rentObjects = new();

        private readonly IFactory<T, TArgs> _factory;
        private readonly TArgs _args;
        private readonly ExpandMode _expandMode;

        public Pool(IFactory<T, TArgs> factory, TArgs args, ExpandMode expandMode = ExpandMode.ExpandByOne)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _args = args ?? throw new ArgumentNullException(nameof(args));
            _expandMode = expandMode;
        }

        public void Init(int initialCount)
        {
            for (int i = 0; i < initialCount; i++)
            {
                T entity = _factory.Create(_args);
                this.OnCreate(entity);
                _pooledObjects.Push(entity);
            }
        }

        public void Dispose()
        {
            foreach (T entity in _pooledObjects)
                this.OnDispose(entity);

            foreach (T entity in _rentObjects)
                this.OnDispose(entity);

            _pooledObjects.Clear();
            _rentObjects.Clear();
        }

        public T Rent()
        {
            if (!_pooledObjects.TryPop(out T entity))
                entity = this.Expand();

            _rentObjects.Add(entity);
            this.OnRent(entity);
            return entity;
        }

        private T Expand()
        {
            switch (_expandMode)
            {
                case ExpandMode.NoExpand:
                    throw new InvalidOperationException(
                        "[Pool] Pool is empty and ExpandMode is NoExpand. " +
                        "Pre-instantiate more entities via Init() or switch to ExpandByOne/ExpandByDoubling.");

                case ExpandMode.ExpandByDoubling:
                    int count = _rentObjects.Count > 0 ? _rentObjects.Count : 1;
                    this.CreateObjects(count);
                    _pooledObjects.TryPop(out T doubled);
                    return doubled;

                case ExpandMode.ExpandByOne:
                default:
                    T entity = _factory.Create(_args);
                    this.OnCreate(entity);
                    return entity;
            }
        }

        private void CreateObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T entity = _factory.Create(_args);
                this.OnCreate(entity);
                _pooledObjects.Push(entity);
            }
        }
        
        public void Return(T entity)
        {
            if (_rentObjects.Remove(entity))
            {
                this.OnReturn(entity);
                _pooledObjects.Push(entity);
            }
            else
            {
#if UNITY_5_3_OR_NEWER
                Debug.LogWarning($"[EntityPool] Attempted to return untracked entity: {entity}");
#endif
            }
        }
        
        protected virtual void OnCreate(T entity)
        {
        }

        protected virtual void OnDispose(T entity)
        {
        }

         protected virtual void OnRent(T entity)
        {
        }

        protected virtual void OnReturn(T entity)
        {
        }
    }
}