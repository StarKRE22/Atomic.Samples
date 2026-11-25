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
    /// manage entity enabling, updating, and cleanup. It wraps a runtime <see cref="EntityManager{E}"/> instance internally.
    /// </remarks>
    /// <example>
    /// Attach this component to a GameObject in the scene to automatically scan and manage entities of type <typeparamref name="E"/>.
    /// </example>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Worlds/MonoEntityWorld%601.md")]
    public abstract class MonoEntityManager<E> : MonoManager<E> where E : IEntity
    {
        protected override Manager<E> Inner => _inner;

        private readonly EntityManager<E> _inner = new();
    }
}
#endif