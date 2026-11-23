using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public bool QueryClosest(Vector2 position, out Vector2 closestPoint, out T closestItem)
        {
            closestPoint = default;
            closestItem = default;

            if (_lookup.Count == 0)
                return false;

            int centerX = Mathf.Clamp(Mathf.FloorToInt((position.x - _offsetX) * _invCellSize), 0, _sizeX - 1);
            int centerY = Mathf.Clamp(Mathf.FloorToInt((position.y - _offsetY) * _invCellSize), 0, _sizeY - 1);

            float bestDistanceSq = float.MaxValue;
            bool found = false;

            int maxRadius = Mathf.Max(_sizeX, _sizeY);
            for (int radius = 0; radius <= maxRadius; radius++)
            {
                int minX = Mathf.Max(0, centerX - radius);
                int maxX = Mathf.Min(_sizeX - 1, centerX + radius);

                int minY = Mathf.Max(0, centerY - radius);
                int maxY = Mathf.Min(_sizeY - 1, centerY + radius);

                for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    bool border = x == minX || x == maxX || y == minY || y == maxY;
                    if (!border)
                        continue;

                    float cellMinX = _offsetX + x * _cellSize;
                    float cellMaxX = cellMinX + _cellSize;

                    float cellMinY = _offsetY + y * _cellSize;
                    float cellMaxY = cellMinY + _cellSize;

                    float dx = 0f;

                    if (position.x < cellMinX)
                        dx = cellMinX - position.x;
                    else if (position.x > cellMaxX)
                        dx = position.x - cellMaxX;

                    float dy = 0f;

                    if (position.y < cellMinY)
                        dy = cellMinY - position.y;
                    else if (position.y > cellMaxY)
                        dy = position.y - cellMaxY;

                    float cellDistanceSq = dx * dx + dy * dy;
                    if (cellDistanceSq >= bestDistanceSq)
                        continue;

                    Cell cell = _cells[x + y * _sizeX];
                    List<KeyValuePair<T, Vector2>> items = cell.items;

                    int count = items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        KeyValuePair<T, Vector2> entry = items[i];

                        float distanceSq = (entry.Value - position).sqrMagnitude;
                        if (distanceSq >= bestDistanceSq)
                            continue;

                        bestDistanceSq = distanceSq;
                        closestPoint = entry.Value;
                        closestItem = entry.Key;
                        found = true;
                    }
                }

                if (found)
                {
                    float nextRingDistance = (radius + 1) * _cellSize;
                    if (bestDistanceSq <= nextRingDistance * nextRingDistance)
                        return true;
                }
            }

            return found;
        }
    }
}