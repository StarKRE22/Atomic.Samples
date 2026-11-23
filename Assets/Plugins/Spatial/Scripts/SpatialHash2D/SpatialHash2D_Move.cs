using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public bool Move(T obj, Vector2 p)
        {
            if (!_lookup.TryGetValue(obj, out Handle handle))
                return false;

            long newKey = GetCellKey(p);
            if (handle.cellKey == newKey)
                return true;

            this.RemoveInternal(handle);

            if (!_cells.TryGetValue(newKey, out List<KeyValuePair<T, Vector2>> newList))
            {
                newList = ListPool<KeyValuePair<T, Vector2>>.Get();
                _cells.Add(newKey, newList);
            }

            int newIndex = newList.Count;
            newList.Add(new KeyValuePair<T, Vector2>(obj, p));

            _lookup[obj] = new Handle(newKey, newIndex);
            return true;
        }
    }
}