using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T> : ISpatialDataStructure2D<T>
    {
        private readonly struct Handle
        {
            public readonly long cellKey;
            public readonly int index;

            public Handle(long cellKey, int index)
            {
                this.cellKey = cellKey;
                this.index = index;
            }
        }

        private readonly float _cellSize;
        private readonly float _invCellSize;

        private int _minCellX, _maxCellX, _minCellY, _maxCellY;
        private bool _boundsDirty;

        private readonly Dictionary<long, List<KeyValuePair<T, Vector2>>> _cells;
        private readonly Dictionary<T, Handle> _lookup;

        public float CellSize => _cellSize;
        public int Count => _lookup.Count;

        public int CellCount => _cells.Count;

        public SpatialHash2D(float cellSize, int capacity = 128)
        {
            _cellSize = cellSize;
            _invCellSize = 1f / cellSize;
            _cells = new Dictionary<long, List<KeyValuePair<T, Vector2>>>(capacity);
            _lookup = new Dictionary<T, Handle>(capacity);
        }

        public bool Contains(T item) => _lookup.ContainsKey(item);

        public int GetCount(Vector2 position)
        {
            long key = this.GetCellKey(position);
            return _cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> entities) ? entities.Count : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetCellKey(Vector2 position)
        {
            int x = Mathf.FloorToInt(position.x * _invCellSize);
            int y = Mathf.FloorToInt(position.y * _invCellSize);

            return GetKey(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetKey(int x, int y) => ((long) x << 32) | (uint) y;

        public IEnumerable<Vector2Int> GetCellPositions()
        {
            foreach (long key in _cells.Keys)
            {
                int x = (int) (key >> 32);
                int y = (int) key;
                yield return new Vector2Int(x, y);
            }
        }
    }
}