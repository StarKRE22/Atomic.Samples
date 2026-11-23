using System;

namespace Atomic.Infrastructure
{
    public interface IPool<T> : IDisposable
    {
        T Rent();

        void Return(T entity);

        void Init(int initialCount);
    }
}