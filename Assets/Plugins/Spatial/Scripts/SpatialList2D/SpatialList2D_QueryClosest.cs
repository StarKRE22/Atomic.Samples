using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public bool QueryClosest(Vector2 position, out Vector2 closestPoint, out T closest)
        {
            if (_count == 0)
            {
                closestPoint = default;
                closest = default;
                return false;
            }

            KeyValuePair<T, Vector2> best = _entries[0];

            float dx = best.Value.x - position.x;
            float dy = best.Value.y - position.y;

            float bestDistanceSq = dx * dx + dy * dy;

            for (int i = 1; i < _count; i++)
            {
                KeyValuePair<T, Vector2> entry = _entries[i];

                dx = entry.Value.x - position.x;
                dy = entry.Value.y - position.y;

                float distanceSq = dx * dx + dy * dy;
                if (distanceSq < bestDistanceSq)
                {
                    best = entry;
                    bestDistanceSq = distanceSq;
                }
            }

            closestPoint = best.Value;
            closest = best.Key;

            return true;
        }
    }
}