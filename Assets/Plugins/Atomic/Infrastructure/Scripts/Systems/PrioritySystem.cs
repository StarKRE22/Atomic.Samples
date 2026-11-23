using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;

namespace Atomic.Infrastructure
{
    public abstract class PrioritySystem<T> : SystemBase<T>, IBudgetSystem
    {
        protected const int DEFAULT_BUDGET = 512;
        protected const float DEFAULT_COOLDOWN = 0.2f;

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

        private readonly Dictionary<T, Entry> _lookup;
        private readonly List<Command> _commands = new(64);
        
        public int HighCount => _highEntityCount;
        public int MidCount => _midEntityCount;
        public int LowCount => _lowEntityCount;


        private T[] _highEntities;
        private T[] _midEntities;
        private T[] _lowEntities;

        private int _highEntityCount;
        private int _midEntityCount;
        private int _lowEntityCount;

        private int _highCursor;
        private int _midCursor;
        private int _lowCursor;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private int _budget;
      
        
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private float _cooldown;
        
        private bool _isUpdating;
        private float _priorityTime;

        private readonly ITracker<T>[] _trackers;

        public int Budget => _budget;
        public int Count => _lookup.Count;

        protected PrioritySystem(
            IReadOnlyStore<T> source,
            int budget = DEFAULT_BUDGET,
            float cooldown = DEFAULT_COOLDOWN,
            params ITracker<T>[] trackers
        ) :
            base(source)
        {
            int capacity = Math.Max(16, source.Count);

            _lookup = new Dictionary<T, Entry>(capacity);

            _highEntities = new T[capacity];
            _midEntities = new T[capacity];
            _lowEntities = new T[capacity];

            _budget = budget;
            _cooldown = cooldown;

            _trackers = trackers ?? Array.Empty<ITracker<T>>();
        }

        public void SetBudget(int budget)
        {
            _budget = Math.Max(0, budget);
        }

        public void SetCooldown(float cooldown)
        {
            _cooldown = cooldown;
        }

        protected override void OnEnable()
        {
            _priorityTime = _cooldown;

            foreach (ITracker<T> tracker in _trackers)
                tracker.SetTrigger(this.RecalculatePriority);
        }

        // ==================== UPDATE ====================

        protected sealed override void OnUpdate(float deltaTime)
        {
            _isUpdating = true;

            try
            {
                this.UpdateCooldown(deltaTime);

                int remaining = _budget;
                remaining -= this.UpdateBatch(_highEntities, _highEntityCount, ref _highCursor, remaining, deltaTime);
                remaining -= this.UpdateBatch(_midEntities, _midEntityCount, ref _midCursor, remaining, deltaTime);
                this.UpdateBatch(_lowEntities, _lowEntityCount, ref _lowCursor, remaining, deltaTime);
            }
            finally
            {
                _isUpdating = false;
                this.UpdateCommands();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int UpdateBatch(T[] entities, int count, ref int cursor, int budget, float deltaTime)
        {
            if (count == 0 || budget <= 0)
                return 0;

            int toProcess = Math.Min(count, budget);
            for (int i = 0; i < toProcess; i++)
            {
                if (cursor >= count)
                    cursor = 0;

                T entity = entities[cursor++];
                this.Update(entity, deltaTime);
            }

            return toProcess;
        }

        protected abstract void Update(T entity, float deltaTime);

        private void UpdateCooldown(float deltaTime)
        {
            if (_cooldown <= 0)
                return;

            _priorityTime -= deltaTime;

            if (_priorityTime > 0)
                return;

            this.RecalculatePriorities();
            _priorityTime = _cooldown;
        }

        protected void RecalculatePriorities()
        {
            foreach (T entity in _store)
                this.RecalculatePriority(entity);
        }

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
        
        protected abstract UpdatePriority EvaluatePriority(T entity);

        private void UpdatePriorityInternal(T entity, UpdatePriority priority)
        {
            if (!_lookup.TryGetValue(entity, out Entry entry))
                return;

            if (entry.Priority == priority)
                return;

            RemoveInternal(entity);
            AddInternal(entity, priority);
        }

        // ==================== ADD ====================

        protected sealed override void OnAdd(T entity)
        {
            if (entity == null)
                throw new Exception("Added null Entity");
            
            if (_lookup.ContainsKey(entity))
                return;

            UpdatePriority priority = EvaluatePriority(entity);

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

        private void AddInternal(T entity, UpdatePriority priority)
        {
            if (_lookup.ContainsKey(entity))
                return;

            switch (priority)
            {
                case Infrastructure.UpdatePriority.High:
                    AddToArray(
                        ref _highEntities,
                        ref _highEntityCount,
                        entity,
                        priority
                    );
                    break;

                case Infrastructure.UpdatePriority.Medium:
                    AddToArray(
                        ref _midEntities,
                        ref _midEntityCount,
                        entity,
                        priority
                    );
                    break;

                case Infrastructure.UpdatePriority.Low:
                default:
                    AddToArray(
                        ref _lowEntities,
                        ref _lowEntityCount,
                        entity,
                        priority
                    );
                    break;
            }
        }

        private void AddToArray(
            ref T[] array,
            ref int count,
            T entity,
            UpdatePriority priority
        )
        {
            if (count >= array.Length)
                Array.Resize(ref array, array.Length * 2);

            array[count] = entity;

            _lookup[entity] = new Entry
            {
                Priority = priority,
                Index = count
            };

            count++;
        }

        // ==================== REMOVE ====================

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
                RemoveInternal(entity);
            }

            foreach (ITracker<T> tracker in _trackers)
                tracker.Untrack(entity);
        }

        private void RemoveInternal(T entity)
        {
            if (!_lookup.TryGetValue(entity, out Entry entry))
                return;

            switch (entry.Priority)
            {
                case Infrastructure.UpdatePriority.High:
                    RemoveFromArray(
                        _highEntities,
                        ref _highEntityCount,
                        ref _highCursor,
                        entry.Index
                    );
                    break;

                case Infrastructure.UpdatePriority.Medium:
                    RemoveFromArray(
                        _midEntities,
                        ref _midEntityCount,
                        ref _midCursor,
                        entry.Index
                    );
                    break;

                case Infrastructure.UpdatePriority.Low:
                default:
                    RemoveFromArray(
                        _lowEntities,
                        ref _lowEntityCount,
                        ref _lowCursor,
                        entry.Index
                    );
                    break;
            }

            _lookup.Remove(entity);
        }

        private void RemoveFromArray(
            T[] array,
            ref int count,
            ref int cursor,
            int index
        )
        {
            int last = count - 1;

            if (index != last)
            {
                T moved = array[last];
                array[index] = moved;

                Entry entry = _lookup[moved];
                entry.Index = index;
                _lookup[moved] = entry;
            }

            array[last] = default;
            count--;

            if (cursor > count)
                cursor = count;
        }

        // ==================== COMMANDS ====================

        private void UpdateCommands()
        {
            foreach (var command in _commands)
            {
                switch (command.CommandType)
                {
                    case Command.Type.Add:
                        AddInternal(command.Entity, command.Priority);
                        break;

                    case Command.Type.Remove:
                        RemoveInternal(command.Entity);
                        break;

                    case Command.Type.Priority:
                        UpdatePriorityInternal(
                            command.Entity,
                            command.Priority
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _commands.Clear();
        }

        // ==================== DISPOSE ====================

        public override void Dispose()
        {
            Array.Clear(_highEntities, 0, _highEntityCount);
            Array.Clear(_midEntities, 0, _midEntityCount);
            Array.Clear(_lowEntities, 0, _lowEntityCount);

            _highEntityCount = 0;
            _midEntityCount = 0;
            _lowEntityCount = 0;

            _highCursor = 0;
            _midCursor = 0;
            _lowCursor = 0;

            _priorityTime = 0;
            _isUpdating = false;

            _commands.Clear();
            _lookup.Clear();
        }
    }
}