using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public class IntervalPrioritySystem<T> : SystemBase<T>
    {
        protected const float DEFAULT_PRIORITY_COOLDOWN = 0.2f;

        #region Configuration

        [Serializable]
        public sealed class Intervals
        {
            [Min(1)]
            public int highInterval = 1;

            [Min(1)]
            public int midInterval = 2;

            [Min(1)]
            public int lowInterval = 4;
        }

        #endregion

        #region Nested Types

        private struct Slot
        {
            public T Entity;
            public double LastUpdateTime;
        }

        private struct Entry
        {
            public Priority Priority;
            public int BucketIndex;
            public int EntityIndex;
        }

        private struct Command
        {
            public enum Type : byte
            {
                Add,
                Remove,
                Priority
            }

            public Type CommandType;
            public T Entity;
            public Priority Priority;
        }

        private sealed class Bucket
        {
            public Slot[] Entities;
            public int Count;

            public Bucket(int capacity)
            {
                Entities = new Slot[capacity];
            }
        }

        private sealed class Group
        {
            public readonly Bucket[] Buckets;

            public int Count;
            private int _cursor;

            public int Interval => Buckets.Length;

            public Group(int interval, int capacity)
            {
                interval = Math.Max(1, interval);

                int bucketCapacity = Math.Max(4, capacity / interval);

                this.Buckets = new Bucket[interval];
                for (int i = 0; i < interval; i++)
                    this.Buckets[i] = new Bucket(bucketCapacity);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int FindSmallestBucket()
            {
                int minIndex = 0;
                int minCount = Buckets[0].Count;

                for (int i = 1; i < Buckets.Length; i++)
                {
                    int count = Buckets[i].Count;

                    if (count < minCount)
                    {
                        minIndex = i;
                        minCount = count;
                    }
                }

                return minIndex;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Bucket GetNextBucket()
            {
                Bucket bucket = Buckets[_cursor++];

                if (_cursor >= Buckets.Length)
                    _cursor = 0;

                return bucket;
            }

            public void Clear()
            {
                foreach (Bucket bucket in Buckets)
                {
                    Array.Clear(bucket.Entities, 0, bucket.Count);
                    bucket.Count = 0;
                }

                _cursor = 0;
                Count = 0;
            }
        }

        #endregion

        private readonly Dictionary<T, Entry> _lookup;
        private readonly List<Command> _commands = new(64);

        private readonly Group _highGroup;
        private readonly Group _midGroup;
        private readonly Group _lowGroup;

        private readonly ITracker<T>[] _trackers;
        private readonly float _priorityCooldown;

        private bool _isUpdating;
        private double _elapsedTime;
        private float _priorityTime;

        #region Properties

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        public int HighEntityCount => _highGroup.Count;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        public int MidEntityCount => _midGroup.Count;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        public int LowEntityCount => _lowGroup.Count;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        public int HighInterval => _highGroup.Interval;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        public int MidInterval => _midGroup.Interval;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        public int LowInterval => _lowGroup.Interval;

        #endregion

        private readonly IJob<T> _job;
        private readonly IPriorityStrategy<T> _priorityStrategy;

        public IntervalPrioritySystem(
            IReadOnlyStore<T> source,
            IJob<T> job,
            IPriorityStrategy<T> priorityStrategy,
            Intervals intervals,
            float priorityCooldown = DEFAULT_PRIORITY_COOLDOWN,
            params ITracker<T>[] trackers
        ) : base(source)
        {
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _priorityStrategy = priorityStrategy ?? throw new ArgumentNullException(nameof(priorityStrategy));


            if (intervals == null)
                throw new ArgumentNullException(nameof(intervals));


            int capacity = Math.Max(16, source.Count);

            _lookup = new Dictionary<T, Entry>(capacity);
            _highGroup = new Group(intervals.highInterval, capacity);
            _midGroup = new Group(intervals.midInterval, capacity);
            _lowGroup = new Group(intervals.lowInterval, capacity);

            _priorityCooldown = priorityCooldown;
            _trackers = trackers ?? Array.Empty<ITracker<T>>();
        }

        protected override void OnEnable()
        {
            _priorityTime = _priorityCooldown;

            foreach (ITracker<T> tracker in _trackers)
                tracker.SetTrigger(this.RecalculatePriority);
        }

        #region Add

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnAdd(T entity)
        {
            Priority priority = _priorityStrategy.Evaluate(entity);

            if (_isUpdating)
            {
                _commands.Add(new Command
                {
                    CommandType = Command.Type.Add,
                    Entity = entity,
                    Priority = priority
                });
            }
            else
            {
                this.AddInternal(entity, priority, _elapsedTime);
            }

            foreach (ITracker<T> tracker in _trackers)
                tracker.Track(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddInternal(T entity, Priority priority, double lastUpdateTime)
        {
            if (_lookup.ContainsKey(entity))
                return;

            Group group = this.GetGroup(priority);
            int bucketIndex = group.FindSmallestBucket();
            Bucket bucket = group.Buckets[bucketIndex];

            if (bucket.Count == bucket.Entities.Length)
                Array.Resize(ref bucket.Entities, bucket.Entities.Length * 2);

            int entityIndex = bucket.Count++;
            bucket.Entities[entityIndex] = new Slot
            {
                Entity = entity,
                LastUpdateTime = lastUpdateTime
            };

            _lookup.Add(entity, new Entry
            {
                Priority = priority,
                BucketIndex = bucketIndex,
                EntityIndex = entityIndex
            });

            group.Count++;
        }

        #endregion

        #region Remove

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnRemove(T entity)
        {
            if (_isUpdating)
            {
                _commands.Add(new Command
                {
                    CommandType = Command.Type.Remove,
                    Entity = entity
                });
            }
            else
            {
                this.RemoveInternal(entity);
            }

            foreach (ITracker<T> tracker in _trackers)
                tracker.Untrack(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveInternal(T entity)
        {
            if (!_lookup.Remove(entity, out Entry entry))
                return;

            Group group = this.GetGroup(entry.Priority);
            Bucket bucket = group.Buckets[entry.BucketIndex];

            int index = entry.EntityIndex;
            int last = --bucket.Count;

            // Swap Back
            if (index != last)
            {
                Slot lastSlot = bucket.Entities[last];
                bucket.Entities[index] = lastSlot;
                Entry lastEntry = _lookup[lastSlot.Entity];
                lastEntry.EntityIndex = index;
                _lookup[lastSlot.Entity] = lastEntry;
            }

            bucket.Entities[last] = default;
            group.Count--;
        }

        #endregion

        #region Update

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnUpdate(float deltaTime)
        {
            _elapsedTime += deltaTime;
            _isUpdating = true;

            try
            {
                this.UpdatePriorityCooldown(deltaTime);
                this.UpdateCommands();

                this.UpdateGroup(_highGroup);
                this.UpdateGroup(_midGroup);
                this.UpdateGroup(_lowGroup);
            }
            finally
            {
                _isUpdating = false;
                this.UpdateCommands();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateGroup(Group group)
        {
            Bucket bucket = group.GetNextBucket();

            Slot[] entities = bucket.Entities;
            int count = bucket.Count;

            for (int i = 0; i < count; i++)
            {
                ref Slot slot = ref entities[i];
                float deltaTime = (float) (_elapsedTime - slot.LastUpdateTime);
                slot.LastUpdateTime = _elapsedTime;

                _job.Execute(slot.Entity, deltaTime);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePriorityCooldown(float deltaTime)
        {
            if (_priorityCooldown <= 0)
                return;

            _priorityTime -= deltaTime;

            if (_priorityTime > 0)
                return;

            this.RecalculatePriorities();

            _priorityTime = _priorityCooldown;
        }

        #endregion

        #region Commands

        private void UpdateCommands()
        {
            foreach (Command command in _commands)
            {
                switch (command.CommandType)
                {
                    case Command.Type.Add:
                        this.AddInternal(command.Entity, command.Priority, _elapsedTime);
                        break;

                    case Command.Type.Remove:
                        this.RemoveInternal(command.Entity);
                        break;

                    case Command.Type.Priority:
                        this.UpdatePriorityInternal(command.Entity, command.Priority);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _commands.Clear();
        }

        #endregion

        #region Priority

        protected void RecalculatePriorities()
        {
            foreach (T entity in _store)
                this.RecalculatePriority(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RecalculatePriority(T entity)
        {
            Priority priority = _priorityStrategy.Evaluate(entity);

            if (_isUpdating)
            {
                _commands.Add(new Command
                {
                    CommandType = Command.Type.Priority,
                    Entity = entity,
                    Priority = priority
                });
            }
            else
            {
                this.UpdatePriorityInternal(entity, priority);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePriorityInternal(T entity, Priority priority)
        {
            if (!_lookup.TryGetValue(entity, out Entry entry))
                return;

            if (entry.Priority == priority)
                return;

            Group group = this.GetGroup(entry.Priority);
            Bucket bucket = group.Buckets[entry.BucketIndex];

            double lastUpdateTime = bucket.Entities[entry.EntityIndex].LastUpdateTime;
            this.RemoveInternal(entity);
            this.AddInternal(entity, priority, lastUpdateTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Group GetGroup(Priority priority)
        {
            return priority switch
            {
                Priority.High => _highGroup,
                Priority.Medium => _midGroup,
                Priority.Low => _lowGroup,
                _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
            };
        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            _highGroup.Clear();
            _midGroup.Clear();
            _lowGroup.Clear();

            _commands.Clear();
            _lookup.Clear();

            _elapsedTime = 0;
            _priorityTime = 0;
            _isUpdating = false;
        }

        #endregion
    }
}