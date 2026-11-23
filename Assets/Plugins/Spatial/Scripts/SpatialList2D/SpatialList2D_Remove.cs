using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public bool Remove(T item)
        {
            if (!_lookup.Remove(item, out int index))
                return false;

            int lastIndex = _count - 1;
            if (index != lastIndex)
            {
                KeyValuePair<T, Vector2> last = _entries[lastIndex];
                _entries[index] = last;
                _lookup[last.Key] = index;
            }

            _entries[lastIndex] = default;
            _count--;

            return true;
        }
    }
}