using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Atomic.Infrastructure
{
    public interface IPrefabPool<T, in U> : IDisposable where U : Component, T
    {
        void Init(U prefab, int count);

        ValueTask InitAsync(U prefab, int initialCount);

        T Rent(U prefab);

        T Rent(U prefab, Transform parent);

        T Rent(U prefab, Vector3 position, Quaternion rotation, Transform parent = null);

        void Return(T entity);

        void Dispose(U prefab);
    }
}