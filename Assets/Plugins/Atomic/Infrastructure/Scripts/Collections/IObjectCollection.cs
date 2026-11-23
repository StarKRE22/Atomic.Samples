using System;
using System.Collections.Generic;

namespace Atomic.Infrastructure
{
    public interface IObjectCollection<T> : IReadOnlyObjectCollection<T>, ICollection<T>, IDisposable
    {
        new int Count { get; }

        new bool Contains(T entity);

        new bool Add(T entity);

        new void CopyTo(T[] array, int arrayIndex);
    }
}