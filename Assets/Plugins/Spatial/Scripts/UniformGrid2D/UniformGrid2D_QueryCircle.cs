using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public int QueryCircle(Vector2 center, float radius, T[] buffer)
        {
            float r2 = radius * radius;
            int count = 0;

            int minX = Mathf.Max(0, Mathf.FloorToInt((center.x - radius - _offsetX) * _invCellSize));
            int maxX = Mathf.Min(_sizeX - 1, Mathf.FloorToInt((center.x + radius - _offsetX) * _invCellSize));
            int minY = Mathf.Max(0, Mathf.FloorToInt((center.y - radius - _offsetY) * _invCellSize));
            int maxY = Mathf.Min(_sizeY - 1, Mathf.FloorToInt((center.y + radius - _offsetY) * _invCellSize));

            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                Cell cell = _cells[x + _sizeX * y];
                List<KeyValuePair<T, Vector2>> items = cell.items;
                int len = items.Count;

                for (int i = 0; i < len; i++)
                {
                    KeyValuePair<T, Vector2> entry = items[i];
                    if ((entry.Value - center).sqrMagnitude > r2)
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