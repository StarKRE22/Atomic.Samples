using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public bool Add(T obj, Vector2 position)
        {
            if (_lookup.ContainsKey(obj))
                return false;

            int x = Mathf.FloorToInt(position.x * _invCellSize);
            int y = Mathf.FloorToInt(position.y * _invCellSize);

            if (_lookup.Count == 0)
            {
                _minCellX = _maxCellX = x;
                _minCellY = _maxCellY = y;
            }
            else
            {
                if (x < _minCellX) _minCellX = x;
                if (x > _maxCellX) _maxCellX = x;

                if (y < _minCellY) _minCellY = y;
                if (y > _maxCellY) _maxCellY = y;
            }

            long key = GetKey(x, y);
            if (!_cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> list))
            {
                list = ListPool<KeyValuePair<T, Vector2>>.Get();
                _cells.Add(key, list);
            }

            int index = list.Count;
            list.Add(new KeyValuePair<T, Vector2>(obj, position));
            _lookup[obj] = new Handle(key, index);

            return true;
        }
    }
}