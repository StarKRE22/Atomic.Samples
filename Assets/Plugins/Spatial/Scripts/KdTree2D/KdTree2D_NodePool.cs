using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        private static class NodePool
        {
            private static readonly Stack<Node> s_pool = new();

            public static Node Rent(Vector2 point, T value)
            {
                if (s_pool.TryPop(out var node))
                {
                    node.point = point;

                    if (node.values == null)
                        node.values = new List<T>();
                    else
                        node.values.Clear();

                    node.values.Add(value);

                    node.left = null;
                    node.right = null;

                    return node;
                }

                return new Node
                {
                    point = point,
                    values = new List<T> { value }
                };
            }

            public static void Return(Node node)
            {
                node.values?.Clear();
                node.left = null;
                node.right = null;

                s_pool.Push(node);
            }
        }
    }
}