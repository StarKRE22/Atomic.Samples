using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public int QueryCircle(Vector2 center, float radius, T[] results)
        {
            float radiusSq = radius * radius;

            int resultCount = 0;
            for (int i = 0; i < _count; i++)
            {
                (T obj, Vector2 position) = _entries[i];

                float dx = position.x - center.x;
                float dy = position.y - center.y;

                float distanceSq = dx * dx + dy * dy;

                if (distanceSq > radiusSq)
                    continue;

                if (resultCount < results.Length)
                    results[resultCount] = obj;

                resultCount++;
            }

            return Mathf.Min(resultCount, results.Length);
        }
    }
}