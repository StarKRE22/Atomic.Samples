#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    public abstract class EntityViewCollection<K, E, V> : ViewCollection<K, E, V>
        where E : class, IEntity
        where V : EntityView<E>
    {
    }
}
#endif