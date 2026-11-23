#if UNITY_EDITOR
using UnityEngine;

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        private const float GIZMOS_Y_OFFSET = 0.05f;

        public void DrawGizmosXZ()
        {
            Gizmos.color = Color.green;
            this.Traverse(DrawNodeXZ);
        }

        private static void DrawNodeXZ(float minX, float minY, float maxX, float maxY, int count)
        {
            Gizmos.color = count switch
            {
                0 => new Color(0, 0, 0, 0),
                < 4 => Color.green,
                < 8 => Color.yellow,
                _ => Color.red
            };

            Vector3 center = new((minX + maxX) * 0.5f, GIZMOS_Y_OFFSET, (minY + maxY) * 0.5f);
            Vector3 size = new(maxX - minX, 0f, maxY - minY);
            Gizmos.DrawCube(center, size);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}