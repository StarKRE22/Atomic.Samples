#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using Atomic.Infrastructure;
using UnityEngine;

namespace Atomic.Entities
{
    
    /// <summary>
    /// A Unity-compatible world manager for scene-based entities of type <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">The specific type of scene entity this world manages. Must inherit from <see cref="MonoEntity"/>.</typeparam>
    /// <remarks>
    /// This component integrates with Unity’s lifecycle events (Awake, Start, OnEnable, etc.) to automatically
    /// manage entity enabling, updating, and cleanup. It wraps a runtime <see cref="EntityWorld{E}"/> instance internally.
    /// </remarks>
    /// <example>
    /// Attach this component to a GameObject in the scene to automatically scan and manage entities of type <typeparamref name="E"/>.
    /// </example>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Worlds/MonoEntityWorld%601.md")]
    public abstract class MonoEntityWorld<E> : MonoObjectWorld<E>, IEntityWorld<E> where E : IEntity
    {
        protected override ObjectWorld<E> Inner => _inner;

        private readonly EntityWorld<E> _inner = new();

        private new void CopyTo(E[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        void IEntityCollection<E>.CopyTo(E[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        
        void ICollection<E>.CopyTo(E[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        void IReadOnlyObjectCollection<E>.CopyTo(E[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        void IObjectCollection<E>.CopyTo(E[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }
    }
}
#endif