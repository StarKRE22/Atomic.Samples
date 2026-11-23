#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityViewStore<K, E, V> : ViewStore<K, E, V>
        where E : class, IEntity
        where V : EntityView<E>
    {
    }
}
#endif