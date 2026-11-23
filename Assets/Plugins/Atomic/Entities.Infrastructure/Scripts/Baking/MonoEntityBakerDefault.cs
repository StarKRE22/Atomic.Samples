using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class MonoEntityBakerDefault<TArgs> : MonoEntityBakerDefault<string, IEntity, EntityView, TArgs>
        where TArgs : IArgs
    {
    }
}