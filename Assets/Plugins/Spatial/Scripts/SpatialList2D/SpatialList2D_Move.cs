using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public bool Move(T item, Vector2 p)
        {
            if (!_lookup.TryGetValue(item, out int index))
                return false;

            KeyValuePair<T, Vector2> pair = _entries[index];
            _entries[index] = new KeyValuePair<T, Vector2>(pair.Key, p);
            return true;
        }
    }
}