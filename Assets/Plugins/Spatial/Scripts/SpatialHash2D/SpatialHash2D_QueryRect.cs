using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public int QueryRect(Rect rect, T[] results)
        {
            int count = 0;

            float xMinRect = rect.xMin;
            float xMaxRect = rect.xMax;
            float yMinRect = rect.yMin;
            float yMaxRect = rect.yMax;

            int minX = Mathf.FloorToInt(xMinRect * _invCellSize);
            int maxX = Mathf.FloorToInt(xMaxRect * _invCellSize);

            int minY = Mathf.FloorToInt(yMinRect * _invCellSize);
            int maxY = Mathf.FloorToInt(yMaxRect * _invCellSize);

            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                long key = GetKey(x, y);
                if (!_cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> list))
                    continue;

                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    (T obj, Vector2 point) = list[i];

                    if (point.x < xMinRect || point.x >= xMaxRect || point.y < yMinRect || point.y >= yMaxRect)
                        continue;

                    if (count >= results.Length)
                        return count;

                    results[count++] = obj;
                }
            }

            return count;
        }
    }
}