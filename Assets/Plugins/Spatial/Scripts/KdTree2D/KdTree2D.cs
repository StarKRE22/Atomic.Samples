using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class KdTree2D<T> : ISpatialDataStructure2D<T>
    {
        internal sealed class Node
        {
            public Vector2 point;
            public List<T> values;
            public Node left, right;
        }

        public enum Axis
        {
            X = 0,
            Y = 1
        }

        internal Node root;

        private int _count;

        public int Count => _count;

        private const float EPS = 0.0001f;
        private readonly Dictionary<T, Node> _lookup;
        
        public KdTree2D(int initialCapacity = 32)
        {
            _lookup = new Dictionary<T, Node>(initialCapacity);
        }

        public bool Contains(T item)
        {
            return _lookup.ContainsKey(item);
        }

        private Node FindNode(Node node, Vector2 point, int depth)
        {
            while (node != null)
            {
                Vector2 delta = point - node.point;

                if (delta.sqrMagnitude <= EPS * EPS)
                    return node;

                Axis axis = (Axis) (depth % 2);

                bool goLeft = axis switch
                {
                    Axis.X => delta.x <= 0f,
                    Axis.Y => delta.y <= 0f,
                    _ => false
                };

                node = goLeft ? node.left : node.right;
                depth++;
            }

            return null;
        }
    }
}