#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;
using UnityEngine;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a scriptable entity aspect that can be applied or discarded on a specific entity type.
    /// </summary>
    /// <typeparam name="E">The type of entity (<see cref="IEntity"/>) this aspect can be applied to.</typeparam>
    /// <remarks>
    /// Inherit from this class to create reusable ScriptableObject assets that encapsulate
    /// logic to apply and discard behaviors or properties on entities.
    /// </remarks>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Aspects/ScriptableEntityAspect%601.md")]
    public abstract class ScriptableEntityAspect<E> : ScriptableAspect<E>, IEntityAspect<E> where E : IEntity
    {
    }
}
#endif