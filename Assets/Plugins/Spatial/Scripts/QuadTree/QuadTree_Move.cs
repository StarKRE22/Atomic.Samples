using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public bool Move(T item, Vector2 p)
        {
            if (!_lookup.TryGetValue(item, out Handle handle))
                return false;

            Node node = handle.node;
            if (p.x >= node.minX && p.x <= node.maxX && p.y >= node.minY && p.y <= node.maxY)
            {
                node.items[handle.index] = new KeyValuePair<T, Vector2>(item, p);
                return true;
            }

            Vector2 oldPosition = node.items[handle.index].Value;
            this.RemoveFromNode(node, handle.index, item);

            if (node.parent != null)
                this.TryMerge(node.parent);

            Handle newHandle = this.AddInternal(_root, item, p);
            if (newHandle.node != null)
            {
                _lookup[item] = newHandle;
                return true;
            }

            Handle rollbackHandle = this.AddInternal(_root, item, oldPosition);
            if (rollbackHandle.node != null)
            {
                _lookup[item] = rollbackHandle;
                return false;
            }

            throw new InvalidOperationException("QuadTree rollback failed.");
        }
    }
}