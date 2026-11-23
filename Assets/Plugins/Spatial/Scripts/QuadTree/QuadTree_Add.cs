using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public bool Add(T item, Vector2 position)
        {
            if (_lookup.ContainsKey(item))
                return false;

            Handle handle = this.AddInternal(_root, item, position);

            if (handle.node == null)
                return false;

            _lookup[item] = handle;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Handle AddInternal(Node node, T item, Vector2 p)
        {
            bool inBounds = p.x >= node.minX && p.x <= node.maxX && p.y >= node.minY && p.y <= node.maxY;
            if (!inBounds)
                return default;

            if (node.divided)
                return this.AddIntoChild(node, item, p);

            if (node.items.Count < _capacity || node.depth >= _maxDepth)
            {
                int index = node.items.Count;
                node.items.Add(new KeyValuePair<T, Vector2>(item, p));

                return new Handle
                {
                    node = node,
                    index = index
                };
            }

            this.Subdivide(node);

            return this.AddIntoChild(node, item, p);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Subdivide(Node node)
        {
            float midX = (node.minX + node.maxX) * 0.5f;
            float midY = (node.minY + node.maxY) * 0.5f;

            node.nw = NodePool.Rent(node.minX, midY, midX, node.maxY, _capacity, node.depth + 1, node);
            node.ne = NodePool.Rent(midX, midY, node.maxX, node.maxY, _capacity, node.depth + 1, node);
            node.sw = NodePool.Rent(node.minX, node.minY, midX, midY, _capacity, node.depth + 1, node);
            node.se = NodePool.Rent(midX, node.minY, node.maxX, midY, _capacity, node.depth + 1, node);

            node.divided = true;

            foreach (KeyValuePair<T, Vector2> entry in node.items)
            {
                Handle handle = this.AddIntoChild(node, entry.Key, entry.Value);
                _lookup[entry.Key] = handle;
            }

            node.items.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Handle AddIntoChild(Node node, T item, Vector2 position)
        {
            float midX = (node.minX + node.maxX) * 0.5f;
            float midY = (node.minY + node.maxY) * 0.5f;

            Node child = position.x < midX
                ? position.y < midY
                    ? node.sw
                    : node.nw
                : position.y < midY
                    ? node.se
                    : node.ne;

            return this.AddInternal(child, item, position);
        }
    }
}