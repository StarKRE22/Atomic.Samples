#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityViewPool<K, E, V> : ViewPool<K, E, V>
        where E : class, IEntity
        where V : EntityView<E>
    {
    }
}
#endif