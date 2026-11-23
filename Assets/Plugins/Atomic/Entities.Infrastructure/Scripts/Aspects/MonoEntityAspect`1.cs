#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;
using UnityEngine;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a scene-based entity aspect that can be applied or discarded on a specific entity type.
    /// </summary>
    /// <typeparam name="E">The type of entity (<see cref="IEntity"/>) this aspect can be applied to.</typeparam>
    /// <remarks>
    /// Inherit from this class to create reusable MonoBehaviour components that encapsulate
    /// logic to apply and discard behaviors or properties on entities during runtime.
    /// Attach this component to a GameObject in your scene to use it.
    /// </remarks>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Aspects/MonoEntityAspect%601.md")]
    public abstract class MonoEntityAspect<E> : MonoAspect<E>, IEntityAspect<E> where E : IEntity
    {
    }
}
#endif