#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;
using UnityEngine;

namespace Atomic.Entities
{
    /// <summary>
    /// A base class for managing collections of entity views in a Unity scene.
    /// Provides functionality to show, hide, add, remove, and clear entity views,
    /// backed by a pool of reusable instances.
    /// </summary>
    /// <typeparam name="K">The key type used to identify entities in the collection.</typeparam>
    /// <typeparam name="E">The type of entity (<see cref="IEntity"/>) managed by this collection.</typeparam>
    /// <typeparam name="V">The type of entity view (<see cref="EntityView{E}"/>) associated with entities.</typeparam>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/UI/EntityWorldView%603.md")]
    public abstract class EntityViewWorld<K, E, V> : ViewWorld<K, E, V>
        where E : class, IEntity
        where V : EntityView<E>
    {
    }
}
#endif