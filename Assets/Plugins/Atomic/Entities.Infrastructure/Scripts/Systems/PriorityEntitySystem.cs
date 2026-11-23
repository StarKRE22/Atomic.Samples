using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Entity system that updates entities according to dynamically assigned priorities.
    /// </summary>
    /// <typeparam name="E">Type of entity processed by the system.</typeparam>
    [Serializable]
    public abstract class PriorityEntitySystem<E> : PrioritySystem<E> where E : IEntity
    {
        protected PriorityEntitySystem(
            IReadOnlyStore<E> source,
            int budget = DEFAULT_BUDGET,
            float cooldown = DEFAULT_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, budget, cooldown, trackers)
        {
        }
    }
}