using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public int QueryPoint(Vector2 point, T[] buffer)
        {
            int count = 0;

            Vector2Int key = GetCell(point);
            if (!_cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> list))
                return 0;

            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                KeyValuePair<T, Vector2> entry = list[i];
                if (entry.Value != point)
                    continue;

                if (count >= buffer.Length)
                    return count;

                buffer[count++] = entry.Key;
            }

            return count;
        }
    }
}