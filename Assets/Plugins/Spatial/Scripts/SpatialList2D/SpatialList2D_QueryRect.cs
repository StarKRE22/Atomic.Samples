using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public int QueryRect(Rect rect, T[] results)
        {
            float xMin = rect.xMin;
            float xMax = rect.xMax;
            float yMin = rect.yMin;
            float yMax = rect.yMax;

            int count = 0;

            for (int i = 0; i < _count && count < results.Length; i++)
            {
                (T key, Vector2 position) = _entries[i];
                if (position.x < xMin || position.x > xMax || position.y < yMin || position.y > yMax)
                    continue;

                results[count++] = key;
            }

            return count;
        }
    }
}