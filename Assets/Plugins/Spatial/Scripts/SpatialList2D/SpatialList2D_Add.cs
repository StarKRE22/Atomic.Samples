using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public bool Add(T item, Vector2 position)
        {
            if (_lookup.ContainsKey(item))
                return false;

            if (_entries == null)
                _entries = new KeyValuePair<T, Vector2>[4];
            else if (_count == _entries.Length)
                System.Array.Resize(ref _entries, _entries.Length * 2);

            int index = _count++;
            _entries[index] = new KeyValuePair<T, Vector2>(item, position);
            _lookup.Add(item, index);
            return true;
        }
    }
}