#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Manages multiple scene entity pools, each associated with a specific prefab.
    /// Provides centralized methods for renting and returning entities across those pools.
    /// </summary>
    /// <typeparam name="E">The type of scene entity being pooled. Must inherit from <see cref="MonoEntity"/>.</typeparam>
    /// <typeparam name="P">The concrete prefab type used to instantiate entities. Must inherit from <see cref="MonoEntity"/> and implement <typeparamref name="E"/>.</typeparam>
    public interface IPrefabEntityPool<E, in P> : IPrefabPool<E, P>
        where E : IEntity
        where P : MonoEntity, E
    {
    }
}
#endif