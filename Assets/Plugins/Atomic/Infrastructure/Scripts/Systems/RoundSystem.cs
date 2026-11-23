using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public abstract class RoundSystem<T> : SystemBase<T>, IBudgetSystem
    {
        protected const int DEFAULT_BUDGET = 1024;

        private enum CommandType : byte
        {
            Add,
            Remove
        }

        private readonly struct Command
        {
            public readonly CommandType Type;
            public readonly T Entity;

            public Command(CommandType type, T entity)
            {
                Type = type;
                Entity = entity;
            }
        }

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private readonly Dictionary<T, int> _lookup;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private T[] _entities;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private int _entityCount;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private int _cursor;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, HideInEditorMode]
#endif
        private int _budget;

        private readonly List<Command> _commands = new();

        private bool _isUpdating;

        public int Budget => _budget;
        public int Count => _entityCount;

        protected RoundSystem(
            IReadOnlyStore<T> source,
            int budget = DEFAULT_BUDGET
        ) : base(source)
        {
            int capacity = Math.Max(4, source.Count);

            _lookup = new Dictionary<T, int>(capacity);
            _entities = new T[capacity];

            _budget = Math.Max(0, budget);
        }

        public void SetBudget(int budget)
        {
            _budget = Math.Max(0, budget);
        }

        public override void Dispose()
        {
            Array.Clear(_entities, 0, _entityCount);

            _lookup.Clear();
            _commands.Clear();

            _entityCount = 0;
            _cursor = 0;
            _isUpdating = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnUpdate(float deltaTime)
        {
            int count = _entityCount;

            if (count == 0 || _budget == 0)
            {
                FlushCommands();
                return;
            }

            int batchSize = Math.Min(count, _budget);
            int cursor = _cursor;

            _isUpdating = true;

            try
            {
                for (int i = 0; i < batchSize; i++)
                {
                    if (cursor >= count)
                        cursor = 0;

                    T entity = _entities[cursor++];

                    Update(entity, deltaTime);
                }
            }
            finally
            {
                _cursor = cursor;
                _isUpdating = false;

                FlushCommands();

                if (_cursor >= _entityCount)
                    _cursor = 0;
            }
        }

        protected abstract void Update(T entity, float deltaTime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnAdd(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_isUpdating)
            {
                _commands.Add(new Command(CommandType.Add, entity));
                return;
            }

            AddInternal(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnRemove(T entity)
        {
            if (entity == null)
                return;

            if (_isUpdating)
            {
                _commands.Add(new Command(CommandType.Remove, entity));
                return;
            }

            RemoveInternal(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddInternal(T entity)
        {
            if (_lookup.ContainsKey(entity))
                return;

            if (_entityCount >= _entities.Length)
            {
                Array.Resize(
                    ref _entities,
                    _entities.Length * 2
                );
            }

            _entities[_entityCount] = entity;
            _lookup[entity] = _entityCount;

            _entityCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveInternal(T entity)
        {
            if (!_lookup.TryGetValue(entity, out int index))
                return;

            int last = _entityCount - 1;

            if (index != last)
            {
                T lastEntity = _entities[last];

                _entities[index] = lastEntity;
                _lookup[lastEntity] = index;
            }

            _entities[last] = default;
            _lookup.Remove(entity);

            _entityCount--;

            if (_cursor > _entityCount)
                _cursor = _entityCount;
        }

        private void FlushCommands()
        {
            if (_commands.Count == 0)
                return;

            try
            {
                for (int i = 0; i < _commands.Count; i++)
                {
                    Command command = _commands[i];

                    switch (command.Type)
                    {
                        case CommandType.Add:
                            AddInternal(command.Entity);
                            break;

                        case CommandType.Remove:
                            RemoveInternal(command.Entity);
                            break;
                    }
                }
            }
            finally
            {
                _commands.Clear();
            }
        }
    }
}