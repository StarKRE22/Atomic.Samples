using System;

namespace Atomic.Infrastructure
{
    public interface IMultiPool<in TKey, T> : IDisposable
    {
        void Init(TKey key, int count);

        T Rent(TKey key);

        void Return(T entity);
    }
}