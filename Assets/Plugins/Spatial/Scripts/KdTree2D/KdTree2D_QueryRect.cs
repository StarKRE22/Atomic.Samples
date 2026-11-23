using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public int QueryRect(Rect rect, T[] results)
        {
            if (root == null || results == null || results.Length == 0)
                return 0;

            int count = 0;
            this.QueryRectRecursive(this.root, rect, 0, results, ref count);
            return count;
        }

        private void QueryRectRecursive(Node node, Rect rect, int depth, T[] results, ref int count)
        {
            if (node == null || count == results.Length)
                return;

            if (rect.Contains(node.point))
            {
                foreach (T value in node.values)
                {
                    results[count++] = value;
                    if (count == results.Length)
                        return;
                }
            }

            Axis axis = (Axis) (depth % 2);
            if (axis == Axis.X)
            {
                if (rect.xMin <= node.point.x)
                    this.QueryRectRecursive(node.left, rect, depth + 1, results, ref count);

                if (rect.xMax >= node.point.x)
                    this.QueryRectRecursive(node.right, rect, depth + 1, results, ref count);
            }
            else
            {
                if (rect.yMin <= node.point.y)
                    this.QueryRectRecursive(node.left, rect, depth + 1, results, ref count);

                if (rect.yMax >= node.point.y)
                    this.QueryRectRecursive(node.right, rect, depth + 1, results, ref count);
            }
        }

        public List<T> QueryRect(Rect rect)
        {
            List<T> result = new();

            if (root != null)
                this.QueryRectRecursive(root, rect, 0, result);

            return result;
        }

        private void QueryRectRecursive(Node node, Rect rect, int depth, List<T> result)
        {
            if (node == null)
                return;

            if (rect.Contains(node.point))
                result.AddRange(node.values);

            Axis axis = (Axis) (depth % 2);
            if (axis == Axis.X)
            {
                if (rect.xMin <= node.point.x)
                    this.QueryRectRecursive(node.left, rect, depth + 1, result);

                if (rect.xMax >= node.point.x)
                    this.QueryRectRecursive(node.right, rect, depth + 1, result);
            }
            else
            {
                if (rect.yMin <= node.point.y)
                    this.QueryRectRecursive(node.left, rect, depth + 1, result);

                if (rect.yMax >= node.point.y)
                    this.QueryRectRecursive(node.right, rect, depth + 1, result);
            }
        }
    }
}