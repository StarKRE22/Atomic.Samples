using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Modules.Spatial
{
    public partial struct UnsafeSpatialHash2D<T> : IDisposable where T : unmanaged, IEquatable<T>
    {
        private readonly struct Handle
        {
            public readonly long cellKey; //O(1) hash
            public readonly int index; // O(1) index aka array

            public Handle(long cellKey, int index)
            {
                this.cellKey = cellKey;
                this.index = index;
            }
        }

        public readonly struct Entry
        {
            public readonly T Item;
            public readonly float2 Position;

            public Entry(T item, float2 position)
            {
                Item = item;
                Position = position;
            }
        }

        private readonly Allocator _allocator;

        private readonly float _cellSize;

        private readonly float _invCellSize;

        private readonly int _initialCellCapacity;

        private UnsafeHashMap<long, UnsafeList<Entry>> _cells;
        private UnsafeHashMap<T, Handle> _lookup;

        public int Count => _lookup.Count;
        public float CellSize => _cellSize;
        public int CellCount => _cells.Count;
        public bool IsCreated => _cells.IsCreated;

        public UnsafeSpatialHash2D(
            Allocator allocator,
            float cellSize = 1,
            int initialCapacity = 128,
            int initialCellCapacity = 4)
        {
            _allocator = allocator;

            _cells = new UnsafeHashMap<long, UnsafeList<Entry>>(initialCapacity, allocator);
            _lookup = new UnsafeHashMap<T, Handle>(initialCapacity, allocator);

            _cellSize = cellSize;
            _invCellSize = 1f / cellSize;

            _initialCellCapacity = initialCellCapacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetKey(float2 position)
        {
            int2 cell = (int2) math.floor(position * _invCellSize);
            return GetKey(cell.x, cell.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetKey(int x, int y)
        {
            return ((long) x << 32) | (uint) y;
        }

        public void Dispose()
        {
            if (!_cells.IsCreated)
                return;

            foreach (KVPair<long, UnsafeList<Entry>> pair in _cells) // Enumerator
            {
                UnsafeList<Entry> cell = pair.Value;
                cell.Dispose();
            }

            _cells.Dispose();
            _lookup.Dispose();
        }

        public NativeArray<long> GetKeys(Allocator allocator)
        {
            return _cells.GetKeyArray(allocator);
        }

        public int GetCount(long key)
        {
            return _cells.TryGetValue(key, out UnsafeList<Entry> cell) && cell.IsCreated ? cell.Length : 0;
        }
    }
}