using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public bool Remove(T item)
        {
            if (!_lookup.TryGetValue(item, out Handle handle))
                return false;

            this.RemoveFromNode(handle.node, handle.index, item);
            
            if (handle.node.parent != null)
                this.TryMerge(handle.node.parent);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveFromNode(Node node, int index, T item)
        {
            int lastIndex = node.items.Count - 1;
            if (index != lastIndex)
            {
                KeyValuePair<T, Vector2> movedEntry = node.items[lastIndex];
                node.items[index] = movedEntry;

                _lookup[movedEntry.Key] = new Handle
                {
                    node = node,
                    index = index
                };
            }

            node.items.RemoveAt(lastIndex);
            _lookup.Remove(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Copy(Node target, Node source)
        {
            foreach (KeyValuePair<T, Vector2> entry in source.items)
            {
                int index = target.items.Count;

                target.items.Add(entry);

                _lookup[entry.Key] = new Handle
                {
                    node = target,
                    index = index
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryMerge(Node node)
        {
            int safety = 0;

            while (node is {divided: true})
            {
                if (++safety > 100)
                    throw new System.Exception("TryMerge infinite loop");

                int total = node.nw.items.Count + node.ne.items.Count + node.sw.items.Count + node.se.items.Count;
                if (total > _capacity)
                    break;

                this.Copy(node, node.nw);
                this.Copy(node, node.ne);
                this.Copy(node, node.sw);
                this.Copy(node, node.se);

                NodePool.Return(node.nw);
                NodePool.Return(node.ne);
                NodePool.Return(node.sw);
                NodePool.Return(node.se);

                node.nw = null;
                node.ne = null;
                node.sw = null;
                node.se = null;

                node.divided = false;

                node = node.parent;
            }
        }
    }
}