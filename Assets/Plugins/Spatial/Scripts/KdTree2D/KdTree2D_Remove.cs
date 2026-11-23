using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public bool Remove(T value)
        {
            if (!_lookup.TryGetValue(value, out Node node))
                return false;

            if (!node.values.Remove(value))
                return false;

            _lookup.Remove(value);

            _count--;

            if (node.values.Count == 0)
                this.RemoveReplacementNode(ref root, node.point, 0);

            return true;
        }


        private bool RemoveReplacementNode(ref Node node, Vector2 point, int depth)
        {
            if (node == null)
                return false;

            Vector2 delta = point - node.point;
            if (delta.sqrMagnitude <= EPS * EPS)
            {
                this.RemoveNode(ref node, depth);
                return true;
            }

            Axis axis = (Axis) (depth % 2);
            bool goLeft = axis switch
            {
                Axis.X => delta.x <= 0f,
                Axis.Y => delta.y <= 0f,
                _ => false
            };

            return goLeft
                ? this.RemoveReplacementNode(ref node.left, point, depth + 1)
                : this.RemoveReplacementNode(ref node.right, point, depth + 1);
        }

        private void RemoveNode(ref Node node, int depth)
        {
            if (node == null)
                return;

            Axis axis = (Axis) (depth % 2);

            // Лист
            if (node.left == null && node.right == null)
            {
                NodePool.Return(node);
                node = null;
                return;
            }

            if (node.right != null)
            {
                Node replacement = FindMin(node.right, axis, depth + 1);

                node.point = replacement.point;
                node.values.Clear();
                node.values.AddRange(replacement.values);

                foreach (T value in node.values)
                    _lookup[value] = node;

                this.RemoveReplacementNode(ref node.right, replacement.point, depth + 1);
            }
            else
            {
                Node replacement = FindMax(node.left, axis, depth + 1);

                node.point = replacement.point;
                node.values.Clear();
                node.values.AddRange(replacement.values);

                foreach (T value in node.values)
                    _lookup[value] = node;

                this.RemoveReplacementNode(ref node.left, replacement.point, depth + 1);
            }
        }
    }
}