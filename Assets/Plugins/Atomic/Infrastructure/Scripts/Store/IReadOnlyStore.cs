using System;
using System.Collections.Generic;

namespace Atomic.Infrastructure
{
    public interface IReadOnlyStore<T> : IReadOnlyCollection<T>
    {
        event Action OnStateChanged;
        event Action<T> OnAdded;
        event Action<T> OnRemoved;

        T this[int index] { get; }

        bool TryGetAt(int index, out T entity);
        bool Contains(T entity);
        void CopyTo(ICollection<T> results);
        void CopyTo(T[] array, int arrayIndex);
    }
}