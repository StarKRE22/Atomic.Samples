using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class QuadTree<T> : ISpatialDataStructure2D<T>
    {
        internal struct Handle
        {
            internal Node node;
            internal int index;
        }
        
        internal sealed class Node
        {
            public float minX, minY, maxX, maxY;
            public int depth;
            
            public Node parent;
            public Node nw, ne, sw, se;
            
            public readonly List<KeyValuePair<T, Vector2>> items;

            public bool divided;

            public Node(float minX, float minY, float maxX, float maxY, int capacity, int depth, Node parent)
            {
                this.minX = minX;
                this.minY = minY;
                this.maxX = maxX;
                this.maxY = maxY;

                this.depth = depth;
                this.parent = parent;

                this.items = new List<KeyValuePair<T, Vector2>>(capacity);
            }
        }

        private Node _root;
        private readonly int _capacity;
        private readonly int _maxDepth;

        private readonly Dictionary<T, Handle> _lookup;

        public int Count => _lookup.Count;
        
        public QuadTree(Vector2 min, Vector2 max, int capacity = 8, int maxDepth = 8)
        {
            _capacity = capacity;
            _maxDepth = maxDepth;

            _root = new Node(min.x, min.y, max.x, max.y, capacity, 0, null);
            _lookup = new Dictionary<T, Handle>(capacity * 4);
        }
        
        public QuadTree(Rect bounds, int capacity = 8, int maxDepth = 8)
        {
            _capacity = capacity;
            _maxDepth = maxDepth;

            _root = new Node(bounds.xMin, bounds.yMin, bounds.xMax, bounds.yMax, capacity, 0, null);
            _lookup = new Dictionary<T, Handle>(capacity * 4);
        }

        public bool Contains(T item) => _lookup.ContainsKey(item);
    }
}