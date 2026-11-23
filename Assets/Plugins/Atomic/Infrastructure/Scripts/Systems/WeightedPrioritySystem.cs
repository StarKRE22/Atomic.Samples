using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    /// <summary>
    /// Entity system that updates entities according to dynamically assigned priorities.
    /// </summary>
    /// <typeparam name="T">Type of entity processed by the system.</typeparam>
    [Serializable]
    public abstract class WeightedPrioritySystem<T> : SystemBase<T>, IBudgetSystem
    {
        protected const int DEFAULT_BUDGET = 512;
        protected const float DEFAULT_COOLDOWN = 0.2f;

        /// <summary>
        /// Configuration for <see cref="WeightedPrioritySystem{T}"/>.
        /// </summary>
        [Serializable]
        public class Distribution
        {
            [Range(0, 100)]
#if ODIN_INSPECTOR
            [OnValueChanged(nameof(Validate))]
#endif
            public int highPercent = 70;

            [Range(0, 100)]
#if ODIN_INSPECTOR
            [OnValueChanged(nameof(Validate))]
#endif
            public int midPercent = 20;

#if ODIN_INSPECTOR
            [ShowInInspector, ReadOnly]
#endif
            public int lowPercent => 100 - highPercent - midPercent;

            public void Validate()
            {
                highPercent = Math.Clamp(highPercent, 0, 100);
                midPercent = Math.Clamp(midPercent, 0, 100 - highPercent);
            }
        }

        private struct Entry
        {
            public UpdatePriority Priority;
            public int Index;
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
            public UpdatePriority Priority;
        }

        private readonly Dictionary<T, Entry> _lookup = new();
        private readonly List<Command> _commands = new(64);

        internal T[] _highEntities;
        internal T[] _midEntities;
        internal T[] _lowEntities;

        internal int _highEntityCount;
        internal int _midEntityCount;
        internal int _lowEntityCount;

        private int _highCursor;
        private int _midCursor;
        private int _lowCursor;

        private bool _isUpdating;

        private float _cooldown;
        private int _budget;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private readonly Distribution _distribution;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private float _priorityTime;

        private readonly ITracker<T>[] _trackers;

        public int Budget => _budget;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedPrioritySystem{T}"/> class.
        /// </summary>
        /// <param name="source">Source collection of entities.</param>
        /// <param name="distribution">System configuration.</param>
        /// <param name="trackers">Priority change triggers.</param>
        protected WeightedPrioritySystem(
            IReadOnlyStore<T> source,
            Distribution distribution,
            int budget = DEFAULT_BUDGET,
            float cooldown = DEFAULT_COOLDOWN,
            params ITracker<T>[] trackers
        ) : base(source)
        {
            int initialCapacity = Math.Max(16, source.Count);
            _highEntities = new T[initialCapacity];
            _midEntities = new T[initialCapacity];
            _lowEntities = new T[initialCapacity];

            _budget = budget;
            _cooldown = cooldown;

            this._distribution = distribution;
            _trackers = trackers;
        }

        public void SetBudget(int budget)
        {
            _budget = budget;
        }

        /// <summary>
        /// Called when the system is enabled.
        /// </summary>
        protected override void OnEnable()
        {
            _priorityTime = _cooldown;

            foreach (ITracker<T> trigger in _trackers)
                trigger.SetTrigger(this.RecalculatePriority);
        }

        #region Add

        /// <summary>
        /// Adds an entity to the system.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnAdd(T entity)
        {
            UpdatePriority priority = this.EvaluatePriority(entity);
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
                AddInternal(entity, priority);
            }

            foreach (ITracker<T> tracker in _trackers)
                tracker.Track(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddInternal(T entity, UpdatePriority priority)
        {
            if (_lookup.ContainsKey(entity))
                return;

            switch (priority)
            {
                case UpdatePriority.High:
                {
                    if (_highEntityCount == _highEntities.Length)
                        Array.Resize(ref _highEntities, _highEntities.Length * 2);

                    _highEntities[_highEntityCount] = entity;
                    _lookup[entity] = new Entry {Priority = priority, Index = _highEntityCount};
                    _highEntityCount++;
                    break;
                }

                case UpdatePriority.Medium:
                {
                    if (_midEntityCount == _midEntities.Length)
                        Array.Resize(ref _midEntities, _midEntities.Length * 2);

                    _midEntities[_midEntityCount] = entity;
                    _lookup[entity] = new Entry {Priority = priority, Index = _midEntityCount};
                    _midEntityCount++;
                    break;
                }

                case UpdatePriority.Low:
                default:
                {
                    if (_lowEntityCount == _lowEntities.Length)
                        Array.Resize(ref _lowEntities, _lowEntities.Length * 2);

                    _lowEntities[_lowEntityCount] = entity;
                    _lookup[entity] = new Entry {Priority = priority, Index = _lowEntityCount};
                    _lowEntityCount++;
                    break;
                }
            }
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes an entity from the system.
        /// </summary>
        /// <param name="entity">Entity to remove.</param>
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

            UpdatePriority p = entry.Priority;
            int index = entry.Index;

            switch (p)
            {
                case UpdatePriority.High:
                {
                    int last = _highEntityCount - 1;
                    if (index != last)
                    {
                        T lastEntity = _highEntities[last];
                        _highEntities[index] = lastEntity;

                        Entry lastEntry = _lookup[lastEntity];
                        lastEntry.Index = index;
                        _lookup[lastEntity] = lastEntry;
                    }

                    _highEntities[last] = default;
                    _highEntityCount--;

                    if (_highCursor >= _highEntityCount)
                        _highCursor = 0;
                    break;
                }

                case UpdatePriority.Medium:
                {
                    int last = _midEntityCount - 1;
                    if (index != last)
                    {
                        T lastEntity = _midEntities[last];
                        _midEntities[index] = lastEntity;

                        Entry lastEntry = _lookup[lastEntity];
                        lastEntry.Index = index;
                        _lookup[lastEntity] = lastEntry;
                    }

                    _midEntities[last] = default;
                    _midEntityCount--;

                    if (_midCursor >= _midEntityCount)
                        _midCursor = 0;
                    break;
                }

                case UpdatePriority.Low:
                default: // Low
                {
                    int last = _lowEntityCount - 1;
                    if (index != last)
                    {
                        T lastEntity = _lowEntities[last];
                        _lowEntities[index] = lastEntity;

                        Entry lastEntry = _lookup[lastEntity];
                        lastEntry.Index = index;
                        _lookup[lastEntity] = lastEntry;
                    }

                    _lowEntities[last] = default;
                    _lowEntityCount--;

                    if (_lowCursor >= _lowEntityCount)
                        _lowCursor = 0;
                    break;
                }
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the system.
        /// </summary>
        /// <param name="batchSize">Maximum number of entities to process.</param>
        /// <param name="deltaTime">Time elapsed since the previous update.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnUpdate(float deltaTime)
        {
            _isUpdating = true;

            try
            {
                this.UpdateCooldown(deltaTime);
                this.UpdateInternal(deltaTime);
            }
            finally
            {
                _isUpdating = false;
                this.UpdateCommands();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCooldown(float deltaTime)
        {
            if (_cooldown <= 0)
                return;

            _priorityTime -= deltaTime;
            if (_priorityTime > 0)
                return;

            this.RecalculatePriorities();
            _priorityTime += _cooldown;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateInternal(float deltaTime)
        {
            _distribution.Validate();

            int highQuota = _budget * _distribution.highPercent / 100;
            int midQuota = _budget * _distribution.midPercent / 100;
            int lowQuota = _budget - highQuota - midQuota;

            // ===== HIGH =====
            int highProcessed = 0;
            if (_highEntityCount > 0 && highQuota > 0)
            {
                int toProcess = Math.Min(highQuota, _highEntityCount);
                int cursor = _highCursor;
                int count = _highEntityCount;
                T[] array = _highEntities;

                for (int i = 0; i < toProcess; i++)
                {
                    if (cursor >= count)
                        cursor = 0;

                    this.Update(array[cursor++], deltaTime);
                }

                _highCursor = cursor;
                highProcessed = toProcess;
            }

            int remaining = highQuota - highProcessed;

            // ===== MID =====
            int midBudget = midQuota + (remaining > 0 ? remaining : 0);
            int midProcessed = 0;

            if (_midEntityCount > 0 && midBudget > 0)
            {
                int toProcess = Math.Min(midBudget, _midEntityCount);
                int cursor = _midCursor;
                int count = _midEntityCount;
                T[] array = _midEntities;

                for (int i = 0; i < toProcess; i++)
                {
                    if (cursor >= count)
                        cursor = 0;

                    this.Update(array[cursor++], deltaTime);
                }

                _midCursor = cursor;
                midProcessed = toProcess;
            }

            remaining = midBudget - midProcessed;

            // ===== LOW =====
            int lowBudget = lowQuota + (remaining > 0 ? remaining : 0);

            if (_lowEntityCount > 0 && lowBudget > 0)
            {
                int toProcess = Math.Min(lowBudget, _lowEntityCount);
                int cursor = _lowCursor;
                int count = _lowEntityCount;
                T[] array = _lowEntities;

                for (int i = 0; i < toProcess; i++)
                {
                    if (cursor >= count)
                        cursor = 0;

                    this.Update(array[cursor++], deltaTime);
                }

                _lowCursor = cursor;
            }

            _isUpdating = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCommands()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _commands.Count; i++)
            {
                var cmd = _commands[i];
                switch (cmd.CommandType)
                {
                    case Command.Type.Add:
                        AddInternal(cmd.Entity, cmd.Priority);
                        break;

                    case Command.Type.Remove:
                        RemoveInternal(cmd.Entity);
                        break;

                    case Command.Type.Priority:
                        UpdatePriorityInternal(cmd.Entity, cmd.Priority);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _commands.Clear();
        }

        /// <summary>
        /// Updates a single entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <param name="deltaTime">Time elapsed since the previous update.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void Update(T entity, float deltaTime);

        #endregion

        #region Priority

        /// <summary>
        /// Evaluates the update priority for the specified entity.
        /// </summary>
        /// <param name="entity">Entity whose priority should be evaluated.</param>
        /// <returns>The priority assigned to the entity.</returns>
        protected abstract UpdatePriority EvaluatePriority(T entity);

        /// <summary>
        /// Recalculates priorities for all tracked entities.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RecalculatePriorities()
        {
            foreach (T entity in _store)
                this.RecalculatePriority(entity);
        }

        /// <summary>
        /// Re-evaluates and updates the priority of the specified entity.
        /// </summary>
        /// <param name="entity">Entity whose priority should be updated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RecalculatePriority(T entity)
        {
            UpdatePriority priority = this.EvaluatePriority(entity);
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
        private void UpdatePriorityInternal(T entity, UpdatePriority priority)
        {
            if (!_lookup.TryGetValue(entity, out var entry))
                return;

            if (entry.Priority == priority)
                return;

            int index = entry.Index;

            // ===== REMOVE =====
            switch (entry.Priority)
            {
                case UpdatePriority.High:
                {
                    int last = _highEntityCount - 1;

                    if (index != last)
                    {
                        T lastEntity = _highEntities[last];
                        _highEntities[index] = lastEntity;

                        var lastEntry = _lookup[lastEntity];
                        lastEntry.Index = index;
                        _lookup[lastEntity] = lastEntry;
                    }

                    _highEntities[last] = default;
                    _highEntityCount--;
                    _highCursor = Math.Min(_highCursor, _highEntityCount);
                    break;
                }

                case UpdatePriority.Medium:
                {
                    int last = _midEntityCount - 1;

                    if (index != last)
                    {
                        T lastEntity = _midEntities[last];
                        _midEntities[index] = lastEntity;

                        var lastEntry = _lookup[lastEntity];
                        lastEntry.Index = index;
                        _lookup[lastEntity] = lastEntry;
                    }

                    _midEntities[last] = default;
                    _midEntityCount--;
                    _midCursor = Math.Min(_midCursor, _midEntityCount);
                    break;
                }

                case UpdatePriority.Low:
                default:
                {
                    int last = _lowEntityCount - 1;

                    if (index != last)
                    {
                        T lastEntity = _lowEntities[last];
                        _lowEntities[index] = lastEntity;

                        var lastEntry = _lookup[lastEntity];
                        lastEntry.Index = index;
                        _lookup[lastEntity] = lastEntry;
                    }

                    _lowEntities[last] = default;
                    _lowEntityCount--;
                    _lowCursor = Math.Min(_lowCursor, _lowEntityCount);
                    break;
                }
            }

            // ===== ADD =====
            switch (priority)
            {
                case UpdatePriority.High:
                {
                    if (_highEntityCount == _highEntities.Length)
                        Array.Resize(ref _highEntities, _highEntities.Length * 2);

                    _highEntities[_highEntityCount] = entity;
                    _lookup[entity] = new Entry {Priority = priority, Index = _highEntityCount};
                    _highEntityCount++;
                    break;
                }

                case UpdatePriority.Medium:
                {
                    if (_midEntityCount == _midEntities.Length)
                        Array.Resize(ref _midEntities, _midEntities.Length * 2);

                    _midEntities[_midEntityCount] = entity;
                    _lookup[entity] = new Entry {Priority = priority, Index = _midEntityCount};
                    _midEntityCount++;
                    break;
                }

                case UpdatePriority.Low:
                default:
                {
                    if (_lowEntityCount == _lowEntities.Length)
                        Array.Resize(ref _lowEntities, _lowEntities.Length * 2);

                    _lowEntities[_lowEntityCount] = entity;
                    _lookup[entity] = new Entry {Priority = priority, Index = _lowEntityCount};
                    _lowEntityCount++;
                    break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Releases all resources used by the system.
        /// </summary>
        public override void Dispose()
        {
            Array.Clear(_lowEntities, 0, _lowEntityCount);
            Array.Clear(_midEntities, 0, _midEntityCount);
            Array.Clear(_highEntities, 0, _highEntityCount);

            _highEntityCount = 0;
            _midEntityCount = 0;
            _lowEntityCount = 0;

            _commands.Clear();
            _lookup.Clear();
        }
    }
}