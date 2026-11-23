using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public bool Add(T obj, Vector2 position)
        {
            if (_lookup.ContainsKey(obj))
                return false;

            if (!this.TryGetIndex(position, out int cellIndex))
                return false;

            ref Cell cell = ref _cells[cellIndex];
            int indexInCell = cell.Add(new KeyValuePair<T, Vector2>(obj, position));

            _lookup[obj] = new Handle
            {
                cellIndex = cellIndex,
                indexInCell = indexInCell
            };

            return true;
        }
    }
}