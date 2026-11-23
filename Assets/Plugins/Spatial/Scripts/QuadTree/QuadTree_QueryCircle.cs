using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public int QueryCircle(Vector2 center, float radius, T[] results)
        {
            int count = 0;

            this.QueryCircleRecursively(
                _root,
                center,
                radius * radius,
                center.x - radius,
                center.y - radius,
                center.x + radius,
                center.y + radius,
                ref count,
                results
            );

            return count;
        }

        private bool QueryCircleRecursively(
            Node node,
            Vector2 center,
            float radiusSquared,
            float minX,
            float minY,
            float maxX,
            float maxY,
            ref int count,
            T[] results
        )
        {
            if (node == null)
                return false;

            if (minX > node.maxX || maxX < node.minX || minY > node.maxY || maxY < node.minY)
                return false;

            foreach (KeyValuePair<T, Vector2> entry in node.items)
            {
                float dx = entry.Value.x - center.x;
                float dy = entry.Value.y - center.y;

                if (dx * dx + dy * dy > radiusSquared)
                    continue;

                if (count >= results.Length)
                    return true;

                results[count++] = entry.Key;
            }

            if (!node.divided)
                return false;

            return this.QueryCircleRecursively(
                       node.nw,
                       center,
                       radiusSquared,
                       minX,
                       minY,
                       maxX,
                       maxY,
                       ref count,
                       results
                   ) ||
                   this.QueryCircleRecursively(
                       node.ne,
                       center,
                       radiusSquared,
                       minX,
                       minY,
                       maxX,
                       maxY,
                       ref count,
                       results
                   ) ||
                   this.QueryCircleRecursively(
                       node.sw,
                       center,
                       radiusSquared,
                       minX,
                       minY,
                       maxX,
                       maxY,
                       ref count,
                       results
                   ) ||
                   this.QueryCircleRecursively(
                       node.se,
                       center,
                       radiusSquared,
                       minX,
                       minY,
                       maxX,
                       maxY,
                       ref count,
                       results
                   );
        }
    }
}