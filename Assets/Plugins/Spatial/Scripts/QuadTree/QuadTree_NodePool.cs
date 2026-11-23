using System.Collections.Generic;

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        private static class NodePool
        {
            private static readonly Stack<Node> _pool = new();

            public static Node Rent(
                float minX,
                float minY,
                float maxX,
                float maxY,
                int capacity,
                int depth,
                Node parent
            )
            {
                if (_pool.TryPop(out Node node))
                {
                    node.minX = minX;
                    node.minY = minY;
                    node.maxX = maxX;
                    node.maxY = maxY;

                    node.depth = depth;
                    node.parent = parent;

                    node.items.Clear();

                    node.nw = null;
                    node.ne = null;
                    node.sw = null;
                    node.se = null;

                    node.divided = false;
                    return node;
                }

                return new Node(minX, minY, maxX, maxY, capacity, depth, parent);
            }

            public static void Return(Node node)
            {
                node.items.Clear();

                node.nw = null;
                node.ne = null;
                node.sw = null;
                node.se = null;

                node.divided = false;

                _pool.Push(node);
            }
        }
    }
}