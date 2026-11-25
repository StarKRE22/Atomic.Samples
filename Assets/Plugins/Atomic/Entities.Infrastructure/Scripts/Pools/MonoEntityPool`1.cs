#if UNITY_5_3_OR_NEWER
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Atomic.Infrastructure;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// A Unity MonoBehaviour-based entity pool for scene-bound entities of type <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">The type of entity managed by this pool. Must inherit from <see cref="MonoEntity"/>.</typeparam>
    /// <typeparam name="U">The concrete prefab type used to instantiate entities. Must inherit from <see cref="MonoEntity"/> and implement <typeparamref name="E"/>.</typeparam>
    /// <remarks>
    /// This pool uses a prefab to instantiate entities and manages their reuse via a stack.
    /// Entities are activated/deactivated on rent/return, and can be preloaded using <see cref="Init(int)"/>.
    /// </remarks>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Pooling/MonoEntityPool%601.md")]
    public abstract class MonoEntityPool<E, U> : MonoPool<E, U>
        where E : IEntity
        where U : MonoEntity, E
    {
        protected override async ValueTask<U[]> CreateAsync(U prefab, int count, Transform container) =>
            await MonoEntity.CreateAsync(prefab, count, container);

        protected override U Create(U prefab, Transform container) =>
            MonoEntity.Create(prefab, container);

        protected override void Destroy(U entity) =>
            MonoEntity.Destroy(entity);

        protected override U Cast(E entity) =>
            MonoEntity.Cast<U>(entity);
    }
}
#endif