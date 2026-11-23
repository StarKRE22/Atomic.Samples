#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public void DrawGizmosXZ(Rect rect)
        {
            Gizmos.color = Color.white;
            this.Traverse(DrawNodeXZ, rect);
        }

        private static void DrawNodeXZ(Vector2 point, int depth, Axis axis, Rect bounds)
        {
            Vector3 worldPoint = new Vector3(point.x, 0f, point.y);

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(worldPoint, 0.15f);

            Handles.Label(worldPoint + new Vector3(0.15f, 0f, 0.15f), $"{depth}");

            if (axis == Axis.X)
                Gizmos.DrawLine(new Vector3(point.x, 0f, bounds.yMin), new Vector3(point.x, 0f, bounds.yMax));
            else
                Gizmos.DrawLine(new Vector3(bounds.xMin, 0f, point.y), new Vector3(bounds.xMax, 0f, point.y));
        }
    }
}
#endif