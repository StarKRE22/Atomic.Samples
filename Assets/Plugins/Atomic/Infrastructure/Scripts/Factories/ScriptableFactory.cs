using UnityEngine;

namespace Atomic.Infrastructure
{
    public abstract class ScriptableFactory<T, TArgs> : ScriptableObject, IFactory<T, TArgs> where TArgs : IArgs
    {
        public abstract T Create(TArgs args);
    }
}