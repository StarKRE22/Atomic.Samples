using System;
using Atomic.Infrastructure;
#if ODIN_INSPECTOR
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// Base implementation of <see cref="EntitySystemBase{E}"/> that maintains
    /// an internal array of active entities and updates them in adaptive batches.
    /// </summary>
    /// <typeparam name="E">Type of entity processed by the system.</typeparam>
    [Serializable]
    public abstract class RoundEntitySystem<E> : RoundSystem<E> where E : IEntity
    {
        protected RoundEntitySystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }
    }
}
