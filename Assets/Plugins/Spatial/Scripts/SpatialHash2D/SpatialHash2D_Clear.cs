using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public void Clear()
        {
            foreach (List<KeyValuePair<T, Vector2>> list in _cells.Values)
                ListPool<KeyValuePair<T, Vector2>>.Release(list);

            _cells.Clear();
            _lookup.Clear();
        }
    }
}