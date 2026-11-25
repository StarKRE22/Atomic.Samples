#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace Atomic.Entities
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that manages a collection of <see cref="EntityView"/>s in the scene.
    /// </summary>
    /// <remarks>
    /// This non-generic version is a concrete specialization of 
    /// <see cref="EntityViewManager{K,E,V}"/> with <typeparamref name="E"/> fixed 
    /// to <see cref="IEntity"/> and <typeparamref name="V"/> fixed to <see cref="EntityView"/>.
    /// </remarks>
    [AddComponentMenu("Atomic/Entities/Entity Collection View")]
    [DisallowMultipleComponent]
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/UI/EntityWorldView.md")]
    public class EntityViewManager : EntityViewManager<string, IEntity, EntityView>
    {
        protected override string GetKey(IEntity entity) => entity.Name;
    }
}
#endif