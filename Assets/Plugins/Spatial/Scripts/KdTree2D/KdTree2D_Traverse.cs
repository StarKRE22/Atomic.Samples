using UnityEngine;
// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public delegate void TraverseHandler(Vector2 point, int depth, Axis axis, Rect bounds);

        public void Traverse(TraverseHandler visitor, Rect bounds)
        {
            this.Traverse(root, 0, bounds, visitor);
        }

        private void Traverse(Node node, int depth, Rect bounds, TraverseHandler handler)
        {
            if (node == null)
                return;

            Axis axis = (Axis) (depth & 1);
            handler(node.point, depth, axis, bounds);

            if (axis == Axis.X)
            {
                Traverse(
                    node.left,
                    depth + 1,
                    Rect.MinMaxRect(bounds.xMin, bounds.yMin, node.point.x, bounds.yMax),
                    handler
                );

                Traverse(
                    node.right,
                    depth + 1,
                    Rect.MinMaxRect(node.point.x, bounds.yMin, bounds.xMax, bounds.yMax),
                    handler
                );
            }
            else
            {
                Traverse(
                    node.left,
                    depth + 1,
                    Rect.MinMaxRect(bounds.xMin, bounds.yMin, bounds.xMax, node.point.y),
                    handler
                );

                Traverse(
                    node.right,
                    depth + 1,
                    Rect.MinMaxRect(bounds.xMin, node.point.y, bounds.xMax, bounds.yMax),
                    handler
                );
            }
        }
    }
}