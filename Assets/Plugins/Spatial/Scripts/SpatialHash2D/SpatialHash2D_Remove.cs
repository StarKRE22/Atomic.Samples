using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public bool Remove(T obj)
        {
            if (!_lookup.TryGetValue(obj, out Handle handle))
                return false;

            this.RemoveInternal(handle);

            _lookup.Remove(obj);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveInternal(Handle handle)
        {
            List<KeyValuePair<T, Vector2>> list = _cells[handle.key];

            int lastIndex = list.Count - 1;
            if (handle.index != lastIndex)
            {
                KeyValuePair<T, Vector2> moved = list[lastIndex];
                list[handle.index] = moved;
                _lookup[moved.Key] = new Handle(handle.key, handle.index);
            }

            list.RemoveAt(lastIndex);

            if (list.Count > 0)
                return;

            _cells.Remove(handle.key);

            int cellX = handle.key.x;
            int cellY = handle.key.y;

            if (cellX == _minCellX || cellX == _maxCellX || cellY == _minCellY || cellY == _maxCellY)
                _boundsDirty = true;

            ListPool<KeyValuePair<T, Vector2>>.Release(list);
        }

        private void RecalculateBounds()
        {
            if (_cells.Count == 0)
            {
                _minCellX = _maxCellX = 0;
                _minCellY = _maxCellY = 0;
                _boundsDirty = false;
                return;
            }

            bool first = true;
            foreach (Vector2Int key in _cells.Keys)
            {
                if (first)
                {
                    _minCellX = _maxCellX = key.x;
                    _minCellY = _maxCellY = key.y;
                    first = false;
                    continue;
                }

                if (key.x < _minCellX)
                    _minCellX = key.x;

                if (key.x > _maxCellX)
                    _maxCellX = key.x;

                if (key.y < _minCellY)
                    _minCellY = key.y;

                if (key.y > _maxCellY)
                    _maxCellY = key.y;
            }

            _boundsDirty = false;
        }
    }
}