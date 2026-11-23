using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public int QueryPoint(Vector2 position, T[] results)
        {
            if (!this.TryGetIndex(position, out int cellIndex))
                return 0;

            Cell cell = _cells[cellIndex];
            List<KeyValuePair<T, Vector2>> items = cell.items;
            
            int count = Mathf.Min(items.Count, results.Length);
            for (int i = 0; i < count; i++)
                results[i] = items[i].Key;

            return count;
        }
    }
}