using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public int QueryRect(Rect rect, T[] results)
        {
            int count = 0;

            float xMinRect = rect.xMin;
            float xMaxRect = rect.xMax;
            float yMinRect = rect.yMin;
            float yMaxRect = rect.yMax;

            int minX = Mathf.FloorToInt((xMinRect - _offsetX) * _invCellSize);
            int maxX = Mathf.FloorToInt((xMaxRect - _offsetX) * _invCellSize);
            int minY = Mathf.FloorToInt((yMinRect - _offsetY) * _invCellSize);
            int maxY = Mathf.FloorToInt((yMaxRect - _offsetY) * _invCellSize);

            minX = Mathf.Clamp(minX, 0, _sizeX - 1);
            maxX = Mathf.Clamp(maxX, 0, _sizeX - 1);
            minY = Mathf.Clamp(minY, 0, _sizeY - 1);
            maxY = Mathf.Clamp(maxY, 0, _sizeY - 1);

            for (int y = minY; y <= maxY; y++)
            {
                int row = y * _sizeX;
                for (int x = minX; x <= maxX; x++)
                {
                    List<KeyValuePair<T, Vector2>> items = _cells[row + x].items;
                    int len = items.Count;

                    for (int i = 0; i < len; i++)
                    {
                        (T obj, Vector2 point) = items[i];
                        if (point.x < xMinRect || point.x >= xMaxRect || point.y < yMinRect || point.y >= yMaxRect)
                            continue;

                        if (count >= results.Length)
                            return count;

                        results[count++] = obj;
                    }
                }
            }

            return count;
        }
    }
}