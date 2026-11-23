using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public int QueryRect(Rect rect, T[] results)
        {
            int count = 0;

            this.QueryRectRecursive(
                _root,
                rect.xMin,
                rect.yMin,
                rect.xMax,
                rect.yMax,
                results,
                ref count
            );

            return count;
        }

        private void QueryRectRecursive(
            Node node,
            float minX,
            float minY,
            float maxX,
            float maxY,
            T[] results,
            ref int count
        )
        {
            if (node == null || count == results.Length)
                return;

            if (minX > node.maxX || maxX < node.minX || minY > node.maxY || maxY < node.minY)
                return;

            foreach ((T key, Vector2 position) in node.items)
            {
                if (position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY)
                {
                    results[count++] = key;
                    if (count == results.Length)
                        return;
                }
            }

            if (node.divided)
            {
                this.QueryRectRecursive(node.nw, minX, minY, maxX, maxY, results, ref count);
                this.QueryRectRecursive(node.ne, minX, minY, maxX, maxY, results, ref count);
                this.QueryRectRecursive(node.sw, minX, minY, maxX, maxY, results, ref count);
                this.QueryRectRecursive(node.se, minX, minY, maxX, maxY, results, ref count);
            }
        }
    }
}