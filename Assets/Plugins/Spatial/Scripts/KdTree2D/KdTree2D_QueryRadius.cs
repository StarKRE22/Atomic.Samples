using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public List<T> QueryCircle(Vector2 target, float radius)
        {
            List<T> result = new List<T>();

            if (root != null && radius > 0f)
                this.QueryCircleRecursive(root, target, radius * radius, 0, result);

            return result;
        }

        private void QueryCircleRecursive(Node node, Vector2 target, float sqrRadius, int depth, List<T> result)
        {
            if (node == null)
                return;

            Vector2 delta = target - node.point;
            if (delta.sqrMagnitude <= sqrRadius + EPS)
                foreach (T t in node.values)
                    result.Add(t);

            Axis axis = (Axis) (depth % 2);
            float axisDiff = axis == Axis.X ? delta.x : delta.y;

            Node primary = axisDiff <= 0f ? node.left : node.right;
            Node secondary = axisDiff <= 0f ? node.right : node.left;

            this.QueryCircleRecursive(primary, target, sqrRadius, depth + 1, result);

            if (axisDiff * axisDiff <= sqrRadius)
                this.QueryCircleRecursive(secondary, target, sqrRadius, depth + 1, result);
        }

        public int QueryCircle(Vector2 target, float radius, T[] results, Predicate<T> predicate)
        {
            if (root == null || radius <= 0f || results == null || results.Length == 0)
                return 0;

            int count = 0;
            this.QueryCircleRecursive(root, target, radius * radius, 0, results, ref count, predicate);
            return count;
        }

        private void QueryCircleRecursive(
            Node node,
            Vector2 target,
            float sqrRadius,
            int depth,
            T[] results,
            ref int count,
            Predicate<T> predicate
        )
        {
            if (node == null || count == results.Length)
                return;

            Vector2 delta = target - node.point;
            if (delta.sqrMagnitude <= sqrRadius + EPS)
            {
                foreach (T v in node.values)
                {
                    if (predicate == null || predicate(v))
                    {
                        results[count++] = v;
                        if (count == results.Length)
                            return;
                    }
                }
            }

            Axis axis = (Axis) (depth % 2);
            float axisDiff = axis == Axis.X ? delta.x : delta.y;

            Node primary = axisDiff <= 0f ? node.left : node.right;
            Node secondary = axisDiff <= 0f ? node.right : node.left;

            this.QueryCircleRecursive(primary, target, sqrRadius, depth + 1, results, ref count, predicate);

            if (axisDiff * axisDiff <= sqrRadius) 
                this.QueryCircleRecursive(secondary, target, sqrRadius, depth + 1, results, ref count, predicate);
        }
        
        public int QueryCircle(Vector2 target, float radius, T[] results)
        {
            if (root == null || radius <= 0f || results == null || results.Length == 0)
                return 0;

            int count = 0;
            this.QueryCircleRecursive(root, target, radius * radius, 0, results, ref count);
            return count;
        }

        private void QueryCircleRecursive(
            Node node,
            Vector2 target,
            float sqrRadius,
            int depth,
            T[] results,
            ref int count
        )
        {
            if (node == null || count == results.Length)
                return;

            Vector2 delta = target - node.point;
            if (delta.sqrMagnitude <= sqrRadius + EPS)
            {
                foreach (T v in node.values)
                {
                    results[count++] = v;
                    if (count == results.Length)
                        return;
                }
            }

            Axis axis = (Axis) (depth % 2);
            float axisDiff = axis == Axis.X ? delta.x : delta.y;
            
            Node primary = axisDiff <= 0f ? node.left : node.right;
            Node secondary = axisDiff <= 0f ? node.right : node.left;

            this.QueryCircleRecursive(primary, target, sqrRadius, depth + 1, results, ref count);

            if (axisDiff * axisDiff <= sqrRadius) 
                this.QueryCircleRecursive(secondary, target, sqrRadius, depth + 1, results, ref count);
        }
    }
}