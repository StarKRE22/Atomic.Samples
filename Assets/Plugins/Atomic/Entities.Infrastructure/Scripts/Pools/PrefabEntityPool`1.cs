#if UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atomic.Infrastructure;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// A multi-prefab object pool for scene-based entities of type <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">The type of <see cref="MonoEntity"/> managed by the pool.</typeparam>
    /// <typeparam name="U">The concrete prefab type used to instantiate entities. Must inherit from <see cref="MonoEntity"/> and implement <typeparamref name="E"/>.</typeparam>
    /// <remarks>
    /// This pool allows renting and returning multiple different entity prefabs, each tracked by its own internal pool.
    /// Pools are created lazily and managed by prefab name. Supports pre-warming via <see cref="Init"/>.
    /// </remarks>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Pooling/PrefabEntityPool%601.md")]
    public abstract class PrefabEntityPool<E, U> : PrefabPool<E, U>
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