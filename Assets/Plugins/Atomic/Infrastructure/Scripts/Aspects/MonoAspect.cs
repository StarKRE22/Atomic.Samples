using UnityEngine;

namespace Atomic.Infrastructure
{
    public abstract class MonoAspect<T> : MonoBehaviour, IAspect<T>
    {
        public abstract void Apply(T item);

        public abstract void Discard(T item);
    }
}