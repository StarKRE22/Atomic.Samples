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
            List<KeyValuePair<T, Vector2>> list = _cells[handle.cellKey];

            int lastIndex = list.Count - 1;
            if (handle.index != lastIndex)
            {
                KeyValuePair<T, Vector2> moved = list[lastIndex];
                list[handle.index] = moved;
                _lookup[moved.Key] = new Handle(handle.cellKey, handle.index);
            }

            list.RemoveAt(lastIndex);

            if (list.Count > 0)
                return;

            _cells.Remove(handle.cellKey);

            int cellX = (int) (handle.cellKey >> 32);
            int cellY = (int) handle.cellKey;

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
            foreach (long key in _cells.Keys)
            {
                int x = (int) (key >> 32);
                int y = (int) key;

                if (first)
                {
                    _minCellX = _maxCellX = x;
                    _minCellY = _maxCellY = y;
                    first = false;
                    continue;
                }

                if (x < _minCellX)
                    _minCellX = x;

                if (x > _maxCellX)
                    _maxCellX = x;

                if (y < _minCellY)
                    _minCellY = y;

                if (y > _maxCellY)
                    _maxCellY = y;
            }

            _boundsDirty = false;
        }
    }
}