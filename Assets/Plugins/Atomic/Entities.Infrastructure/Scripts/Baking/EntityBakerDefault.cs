using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityBakerDefault<TArgs> : EntityBakerDefault<string, IEntity, EntityView, TArgs>
        where TArgs : IArgs
    {
    }
}