using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T> : ISpatialDataStructure2D<T>
    {
        private KeyValuePair<T, Vector2>[] _entries;
        private readonly Dictionary<T, int> _lookup = new();
        private int _count;

        public int Count => _count;
        
        public SpatialList2D(int capacity = 4)
        {
            _entries = new KeyValuePair<T, Vector2>[capacity];
        }

        public bool Contains(T item) => _lookup.ContainsKey(item);

        public void Clear()
        {
            if (_entries != null)
                System.Array.Clear(_entries, 0, _count);

            _lookup.Clear();
            _count = 0;
        }
    }
}