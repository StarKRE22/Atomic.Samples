using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public int QueryCircle(Vector2 center, float radius, T[] buffer)
        {
            int count = 0;

            int minX = Mathf.FloorToInt((center.x - radius) * _invCellSize);
            int maxX = Mathf.FloorToInt((center.x + radius) * _invCellSize);

            int minY = Mathf.FloorToInt((center.y - radius) * _invCellSize);
            int maxY = Mathf.FloorToInt((center.y + radius) * _invCellSize);

            float sqrRadius = radius * radius;
            
            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                long key = GetKey(x, y);
                if (!_cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> list))
                    continue;

                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    KeyValuePair<T, Vector2> entry = list[i];
                    Vector2 delta = entry.Value - center;

                    if (delta.sqrMagnitude > sqrRadius)
                        continue;

                    if (count >= buffer.Length)
                        return count;

                    buffer[count++] = entry.Key;
                }
            }

            return count;
        }
    }
}