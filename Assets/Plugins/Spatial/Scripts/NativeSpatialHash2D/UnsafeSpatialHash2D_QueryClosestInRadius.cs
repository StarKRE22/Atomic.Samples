using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Modules.Spatial
{
    public partial struct UnsafeSpatialHash2D<T>
    {
        public bool QueryClosestInRadius(
            float2 center,
            float radius,
            out float2 closestPoint,
            out T closest)
        {
            closestPoint = default;
            closest = default;

            if (_lookup.Count == 0)
                return false;

            float radiusSq = radius * radius;

            int minX = (int) math.floor((center.x - radius) * _invCellSize);
            int maxX = (int) math.floor((center.x + radius) * _invCellSize);
            int minY = (int) math.floor((center.y - radius) * _invCellSize);
            int maxY = (int) math.floor((center.y + radius) * _invCellSize);

            float bestDistanceSq = radiusSq;
            bool found = false;

            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                long key = GetKey(x, y);
                if (!_cells.TryGetValue(key, out UnsafeList<Entry> cell))
                    continue;

                foreach (Entry entry in cell)
                {
                    float distanceSq = math.lengthsq(entry.Position - center);
                    if (distanceSq > bestDistanceSq)
                        continue;

                    bestDistanceSq = distanceSq;
                    closestPoint = entry.Position;
                    closest = entry.Item;
                    found = true;
                }
            }

            return found;
        }
    }
}