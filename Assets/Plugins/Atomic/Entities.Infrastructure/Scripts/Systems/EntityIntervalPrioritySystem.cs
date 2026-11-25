using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityIntervalPrioritySystem : EntityIntervalPrioritySystem<IEntity>
    {
        protected EntityIntervalPrioritySystem(
            IReadOnlyStore<IEntity> source,
            Intervals intervals,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<IEntity>[] trackers
        ) : base(source, intervals, cooldown, trackers)
        {
        }
    }

    public abstract class EntityIntervalPrioritySystem<E> : IntervalPrioritySystem<E> where E : IEntity
    {
        protected EntityIntervalPrioritySystem(
            IReadOnlyStore<E> source,
            Intervals intervals,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, intervals, cooldown, trackers)
        {
        }
    }

    #region Tick

    public abstract class EntityTickIntervalPrioritySystem : EntityTickIntervalPrioritySystem<IEntity>
    {
        protected EntityTickIntervalPrioritySystem(
            IReadOnlyStore<IEntity> source,
            Intervals intervals,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<IEntity>[] trackers
        ) : base(source, intervals, cooldown, trackers)
        {
        }
    }

    public abstract class EntityTickIntervalPrioritySystem<E> : EntityIntervalPrioritySystem<E> where E : IEntity
    {
        protected EntityTickIntervalPrioritySystem(
            IReadOnlyStore<E> source,
            Intervals intervals,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, intervals, cooldown, trackers)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.Tick(deltaTime);
    }

    #endregion

    #region FixedTick

    public class EntityFixedTickIntervalPrioritySystem
        : EntityFixedTickIntervalPrioritySystem<IEntity>
    {
        public EntityFixedTickIntervalPrioritySystem(
            IReadOnlyStore<IEntity> source,
            Intervals intervals,
            Func<IEntity, UpdatePriority> priorityEvaluator,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<IEntity>[] trackers
        ) : base(source, intervals, priorityEvaluator, cooldown, trackers)
        {
        }
    }

    public class EntityFixedTickIntervalPrioritySystem<E>
        : EntityIntervalPrioritySystem<E> where E : IEntity
    {
        public EntityFixedTickIntervalPrioritySystem(
            IReadOnlyStore<E> source,
            Intervals intervals,
            Func<E, UpdatePriority> priorityEvaluator,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, intervals, priorityEvaluator, cooldown, trackers)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.FixedTick(deltaTime);
    }

    #endregion

    #region LateTick

    public class EntityLateTickIntervalPrioritySystem
        : EntityLateTickIntervalPrioritySystem<IEntity>
    {
        public EntityLateTickIntervalPrioritySystem(
            IReadOnlyStore<IEntity> source,
            Intervals intervals,
            Func<IEntity, UpdatePriority> priorityEvaluator,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<IEntity>[] trackers
        ) : base(source, intervals, priorityEvaluator, cooldown, trackers)
        {
        }
    }

    public class EntityLateTickIntervalPrioritySystem<E>
        : EntityIntervalPrioritySystem<E> where E : IEntity
    {
        public EntityLateTickIntervalPrioritySystem(
            IReadOnlyStore<E> source,
            Intervals intervals,
            Func<E, UpdatePriority> priorityEvaluator,
            float cooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<E>[] trackers
        ) : base(source, intervals, priorityEvaluator, cooldown, trackers)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.LateTick(deltaTime);
    }

    #endregion
}