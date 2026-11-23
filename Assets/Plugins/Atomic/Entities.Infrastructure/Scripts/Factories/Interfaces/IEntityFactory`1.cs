using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Defines a generic factory interface for creating new instances of <see cref="IEntity"/>-based types.
    /// 
    /// This interface is typically implemented by systems or data-driven structures (e.g., ScriptableObjects, MonoBehaviours)
    /// that are responsible for instantiating and configuring entities at runtime.
    /// </summary>
    /// <typeparam name="E">The type of <see cref="IEntity"/> this factory creates.</typeparam>
    /// <typeparam name="TArgs">The type of construction arguments passed to the factory. Must implement <see cref="IArgs"/>.</typeparam>
    public interface IEntityFactory<out E, in TArgs> : IFactory<E, TArgs> where E : IEntity where TArgs : IArgs
    {
    }
}