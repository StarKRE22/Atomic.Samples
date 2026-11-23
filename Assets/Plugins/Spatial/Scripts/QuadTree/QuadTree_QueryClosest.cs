using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public bool QueryClosest(Vector2 position, out Vector2 closestPoint, out T closestItem)
        {
            if (_lookup.Count == 0)
            {
                closestPoint = default;
                closestItem = default;
                return false;
            }

            KeyValuePair<T, Vector2> bestEntry = default;
            float bestDistance = float.MaxValue;

            this.QueryClosestRecursive(
                _root,
                position,
                ref bestEntry,
                ref bestDistance
            );

            closestPoint = bestEntry.Value;
            closestItem = bestEntry.Key;

            return true;
        }

        private void QueryClosestRecursive(
            Node node,
            Vector2 target,
            ref KeyValuePair<T, Vector2> bestEntry,
            ref float bestDistance
        )
        {
            if (node == null)
                return;

            float nodeDistance = this.GetDistanceToBoundsSquared(node, target);
            if (nodeDistance > bestDistance)
                return;

            foreach (KeyValuePair<T, Vector2> entry in node.items)
            {
                float dx = entry.Value.x - target.x;
                float dy = entry.Value.y - target.y;
                float distance = dx * dx + dy * dy;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestEntry = entry;
                }
            }

            if (node.divided)
            {
                this.QueryClosestRecursive(node.nw, target, ref bestEntry, ref bestDistance);
                this.QueryClosestRecursive(node.ne, target, ref bestEntry, ref bestDistance);
                this.QueryClosestRecursive(node.sw, target, ref bestEntry, ref bestDistance);
                this.QueryClosestRecursive(node.se, target, ref bestEntry, ref bestDistance);
            }
        }

        private float GetDistanceToBoundsSquared(Node node, Vector2 point)
        {
            float dx = 0f;

            if (point.x < node.minX)
                dx = node.minX - point.x;
            else if (point.x > node.maxX)
                dx = point.x - node.maxX;

            float dy = 0f;

            if (point.y < node.minY)
                dy = node.minY - point.y;
            else if (point.y > node.maxY)
                dy = point.y - node.maxY;

            return dx * dx + dy * dy;
        }
    }
}