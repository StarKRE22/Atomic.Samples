using System;

namespace Atomic.Infrastructure
{
    public interface ITracker<E>
    {
        void Track(E entity);

        void Untrack(E entity);

        void SetTrigger(Action<E> action);
    }
}