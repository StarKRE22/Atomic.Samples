using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public bool Add(T value, Vector2 point)
        {
            if (!_lookup.TryAdd(value, null))
                return false;

            bool success = AddRecursive(ref root, point, value, 0);

            if (!success)
                _lookup.Remove(value);

            return success;
        }

        private bool AddRecursive(ref Node node, Vector2 point, T value, int depth)
        {
            if (node == null)
            {
                node = NodePool.Rent(point, value);

                _lookup[value] = node;

                _count++;
                return true;
            }

            Vector2 delta = point - node.point;

            if (delta.sqrMagnitude <= EPS * EPS)
            {
                node.values.Add(value);

                _lookup[value] = node;

                _count++;
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
                ? this.AddRecursive(ref node.left, point, value, depth + 1)
                : this.AddRecursive(ref node.right, point, value, depth + 1);
        }
    }
}