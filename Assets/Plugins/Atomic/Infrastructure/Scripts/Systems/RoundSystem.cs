using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;

namespace Atomic.Infrastructure
{
    [Serializable]
    public abstract class RoundSystem<T> : SystemBase<T>, IBudgetSystem
    {
        protected const int DEFAULT_BUDGET = 256;
        
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

        public int Budget => _budget;
        
        public int Count => _entityCount;

        protected RoundSystem(IReadOnlyStore<T> source, int budget = DEFAULT_BUDGET) : base(source)
        {
            int capacity = Math.Max(4, source.Count);

            _lookup = new Dictionary<T, int>(capacity);
            _entities = new T[capacity];
            _budget = budget;
        }

        public void SetBudget(int budget)
        {
            _budget = Math.Max(0, budget);
        }

        public override void Dispose()
        {
            Array.Clear(_entities, 0, _entityCount);
            _lookup.Clear();

            _entityCount = 0;
            _cursor = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnUpdate(float deltaTime)
        {
            int count = _entityCount;
            if (count == 0 || _budget == 0)
                return;

            int batchSize = Math.Min(count, _budget);
            int cursor = _cursor;

            for (int i = 0; i < batchSize; i++)
            {
                if (cursor >= count)
                    cursor = 0;

                T entity = _entities[cursor++];
                this.Update(entity, deltaTime);
            }

            _cursor = cursor;
        }

        protected abstract void Update(T entity, float deltaTime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnAdd(T entity)
        {
            if (_lookup.ContainsKey(entity))
                return;

            if (_entityCount >= _entities.Length)
                Array.Resize(ref _entities, _entities.Length * 2);

            _entities[_entityCount] = entity;
            _lookup[entity] = _entityCount;

            _entityCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void OnRemove(T entity)
        {
            if (!_lookup.TryGetValue(entity, out int index))
                return;

            int last = _entityCount - 1;

            if (index != last)
            {
                _entities[index] = _entities[last];
                _lookup[_entities[index]] = index;
            }

            _entities[last] = default;
            _lookup.Remove(entity);

            _entityCount--;

            // Keep the cursor within valid bounds.
            if (_cursor > _entityCount)
                _cursor = _entityCount;
        }
    }
}