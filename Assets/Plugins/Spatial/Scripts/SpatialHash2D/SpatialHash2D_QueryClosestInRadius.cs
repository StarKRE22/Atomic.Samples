using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public bool QueryClosestInRadius(Vector2 center, float radius, out Vector2 closestPoint, out T closest)
        {
            closestPoint = default;
            closest = default;

            if (_lookup.Count == 0)
                return false;

            float radiusSq = radius * radius;

            int minX = Mathf.FloorToInt((center.x - radius) * _invCellSize);
            int maxX = Mathf.FloorToInt((center.x + radius) * _invCellSize);
            int minY = Mathf.FloorToInt((center.y - radius) * _invCellSize);
            int maxY = Mathf.FloorToInt((center.y + radius) * _invCellSize);

            float bestDistanceSq = radiusSq;
            bool found = false;

            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                Vector2Int key = new Vector2Int(x, y);
                if (!_cells.TryGetValue(key, out List<KeyValuePair<T, Vector2>> list))
                    continue;

                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    KeyValuePair<T, Vector2> entry = list[i];

                    float dx = entry.Value.x - center.x;
                    float dy = entry.Value.y - center.y;

                    float distanceSq = dx * dx + dy * dy;
                    if (distanceSq > radiusSq)
                        continue;

                    if (distanceSq >= bestDistanceSq)
                        continue;

                    bestDistanceSq = distanceSq;
                    closestPoint = entry.Value;
                    closest = entry.Key;
                    found = true;
                }
            }

            return found;
        }
    }
}