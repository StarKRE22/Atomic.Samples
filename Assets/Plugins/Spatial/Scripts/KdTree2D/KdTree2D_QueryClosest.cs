using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public bool QueryClosest(Vector2 position, out Vector2 closestPoint, out T closestItem)
        {
            if (root == null)
            {
                closestPoint = default;
                closestItem = default;
                return false;
            }

            Node bestNode = null;
            float bestDist = float.MaxValue;

            this.QueryClosestRecursive(root, position, ref bestNode, ref bestDist, 0);

            closestPoint = bestNode.point;
            List<T> values = bestNode.values;
            closestItem = values[0];

            return true;
        }

        private void QueryClosestRecursive(Node node, Vector2 target, ref Node best, ref float bestDist, int depth)
        {
            if (node == null)
                return;

            Vector2 delta = target - node.point;

            float distance = delta.sqrMagnitude;
            if (distance < bestDist)
            {
                best = node;
                bestDist = distance;
            }

            Axis axis = (Axis) (depth % 2);
            float axisDiff = axis == Axis.X ? delta.x : delta.y;

            Node primary = axisDiff <= 0f ? node.left : node.right;
            Node secondary = axisDiff <= 0f ? node.right : node.left;

            this.QueryClosestRecursive(primary, target, ref best, ref bestDist, depth + 1);

            if (axisDiff * axisDiff > bestDist)
                return;

            this.QueryClosestRecursive(secondary, target, ref best, ref bestDist, depth + 1);
        }
    }
}