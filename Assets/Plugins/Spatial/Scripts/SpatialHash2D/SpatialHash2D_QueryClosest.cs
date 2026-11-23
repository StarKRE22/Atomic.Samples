using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public bool QueryClosest(Vector2 center, out Vector2 closestPoint, out T closest)
        {
            if (_boundsDirty)
                this.RecalculateBounds();

            closestPoint = default;
            closest = default;

            if (_lookup.Count == 0)
                return false;

            int centerX = Mathf.FloorToInt(center.x * _invCellSize);
            int centerY = Mathf.FloorToInt(center.y * _invCellSize);

            float bestDistanceSq = float.MaxValue;
            bool found = false;

            int maxRadius = Mathf.Max(
                Mathf.Max(Mathf.Abs(centerX - _minCellX), Mathf.Abs(centerX - _maxCellX)),
                Mathf.Max(Mathf.Abs(centerY - _minCellY), Mathf.Abs(centerY - _maxCellY))
            );

            for (int radius = 0; radius <= maxRadius; radius++)
            {
                int minX = centerX - radius;
                int maxX = centerX + radius;
                int minY = centerY - radius;
                int maxY = centerY + radius;

                for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    bool border = x == minX || x == maxX || y == minY || y == maxY;
                    if (!border)
                        continue;

                    float cellMinX = x * _cellSize;
                    float cellMaxX = cellMinX + _cellSize;

                    float cellMinY = y * _cellSize;
                    float cellMaxY = cellMinY + _cellSize;

                    float dx = 0f;

                    if (center.x < cellMinX) 
                        dx = cellMinX - center.x;
                    else if (center.x > cellMaxX)
                        dx = center.x - cellMaxX;

                    float dy = 0f;

                    if (center.y < cellMinY) 
                        dy = cellMinY - center.y;
                    else if (center.y > cellMaxY) 
                        dy = center.y - cellMaxY;

                    float cellDistanceSq = dx * dx + dy * dy;
                    if (cellDistanceSq >= bestDistanceSq)
                        continue;

                    if (!_cells.TryGetValue(GetKey(x, y), out List<KeyValuePair<T, Vector2>> list))
                        continue;

                    int count = list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        KeyValuePair<T, Vector2> entry = list[i];

                        dx = entry.Value.x - center.x;
                        dy = entry.Value.y - center.y;

                        float distanceSq = dx * dx + dy * dy;
                        if (distanceSq >= bestDistanceSq)
                            continue;

                        bestDistanceSq = distanceSq;
                        closestPoint = entry.Value;
                        closest = entry.Key;
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