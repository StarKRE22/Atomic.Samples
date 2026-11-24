using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T> : ISpatialDataStructure2D<T>
    {
        private readonly struct Handle
        {
            public readonly Vector2Int key;
            public readonly int index;

            public Handle(Vector2Int key, int index)
            {
                this.key = key;
                this.index = index;
            }
        }

        private readonly float _cellSize;
        private readonly float _invCellSize;

        private int _minCellX, _maxCellX, _minCellY, _maxCellY;
        private bool _boundsDirty;

        private readonly Dictionary<Vector2Int, List<KeyValuePair<T, Vector2>>> _cells;
        private readonly Dictionary<T, Handle> _lookup;

        public float CellSize => _cellSize;
        public int Count => _lookup.Count;

        public int CellCount => _cells.Count;

        public SpatialHash2D(float cellSize, int capacity = 128)
        {
            _cellSize = cellSize;
            _invCellSize = 1f / cellSize;
            _cells = new Dictionary<Vector2Int, List<KeyValuePair<T, Vector2>>>(capacity);
            _lookup = new Dictionary<T, Handle>(capacity);
        }

        public bool Contains(T item) => _lookup.ContainsKey(item);

        public int GetCount(Vector2 position)
        {
            Vector2Int key = this.GetCell(position);
            return _cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> entities) ? entities.Count : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2Int GetCell(Vector2 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x * _invCellSize),
                Mathf.FloorToInt(position.y * _invCellSize)
            );
        }
    }
}