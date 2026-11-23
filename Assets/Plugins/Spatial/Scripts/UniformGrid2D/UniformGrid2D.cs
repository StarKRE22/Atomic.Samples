using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T> : ISpatialDataStructure2D<T>
    {
        private struct Handle
        {
            public int cellIndex;
            public int indexInCell;
        }

        internal readonly struct Cell
        {
            public int Count => items.Count;

            internal readonly List<KeyValuePair<T, Vector2>> items;

            internal Cell(int capacity) =>
                items = new List<KeyValuePair<T, Vector2>>(capacity);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal int Add(in KeyValuePair<T, Vector2> entry)
            {
                int index = items.Count;
                items.Add(entry);
                return index;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void SetPosition(int index, Vector2 position)
            {
                KeyValuePair<T, Vector2> entry = items[index];
                items[index] = new KeyValuePair<T, Vector2>(entry.Key, position);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void RemoveAtSwapBack(int index, out KeyValuePair<T, Vector2> moved)
            {
                int last = items.Count - 1;

                moved = items[last];

                if (index != last)
                    items[index] = moved;

                items.RemoveAt(last);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Clear()
            {
                items.Clear();
            }
        }

        private readonly float _cellSize;
        private readonly float _invCellSize;

        private readonly int _sizeX;
        private readonly int _sizeY;

        private readonly float _offsetX;
        private readonly float _offsetY;

        private readonly Cell[] _cells;
        private readonly Dictionary<T, Handle> _lookup;

        public int Count => _lookup.Count;

        public int SizeX => _sizeX;
        public int SizeY => _sizeY;
        public float CellSize => _cellSize;

        public float OffsetX => _offsetX;
        public float OffsetY => _offsetY;

        public UniformGrid2D(Vector2Int size, Vector2 offset, float cellSize, int cellCapacity = 4) :
            this(size.x, size.y, cellSize, cellCapacity, offset.x, offset.y)
        {
        }

        public UniformGrid2D(
            int sizeX,
            int sizeY,
            float cellSize,
            int cellCapacity = 4,
            float offsetX = 0,
            float offsetY = 0
        )
        {
            _sizeX = sizeX;
            _sizeY = sizeY;

            _offsetX = offsetX;
            _offsetY = offsetY;

            _cellSize = cellSize;
            _invCellSize = 1f / cellSize;

            int total = sizeX * sizeY;

            _cells = new Cell[total];
            for (int i = 0; i < total; i++)
                _cells[i] = new Cell(cellCapacity);

            _lookup = new Dictionary<T, Handle>(total);
        }

        public T this[int x, int y, int index]
        {
            get
            {
                ref Cell cell = ref _cells[x + _sizeX * y];
                List<KeyValuePair<T, Vector2>> items = cell.items;
                KeyValuePair<T, Vector2> entry = items[index];
                return entry.Key;
            }
        }

        public T GetValue(int x, int y, int index)
        {
            ref Cell cell = ref _cells[x + _sizeX * y];
            List<KeyValuePair<T, Vector2>> items = cell.items;
            KeyValuePair<T, Vector2> entry = items[index];
            return entry.Key;
        }

        public bool Contains(T item) => _lookup.ContainsKey(item);

        public int GetCount(int x, int y) =>
            x < 0 || x >= _sizeX || y < 0 || y >= _sizeY ? 0 : _cells[x + _sizeX * y].Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetIndex(Vector2 pos, out int index)
        {
            int x = Mathf.FloorToInt((pos.x - _offsetX) * _invCellSize);
            int y = Mathf.FloorToInt((pos.y - _offsetY) * _invCellSize);

            if (x < 0 || x >= _sizeX || y < 0 || y >= _sizeY)
            {
                index = -1;
                return false;
            }

            index = x + _sizeX * y;
            return true;
        }
    }
}