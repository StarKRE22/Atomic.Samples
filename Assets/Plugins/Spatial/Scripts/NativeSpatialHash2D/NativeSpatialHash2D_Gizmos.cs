#if UNITY_EDITOR
using Unity.Collections;
using UnityEngine;

namespace Modules.Spatial
{
    public partial struct NativeSpatialHash2D<T>
    {
        private const float GIZMOS_Y_OFFSET = 0.05f;

        public void DrawGizmosXZ()
        {
            float cellSize = _hash.CellSize;
            foreach (long key in this.GetKeys(Allocator.Temp))
            {
                int count = this.GetCount(key);
                int x = (int) (key >> 32);
                int y = (int) key;

                Vector3 center = new Vector3((x + 0.5f) * cellSize, GIZMOS_Y_OFFSET, (y + 0.5f) * cellSize);
                Vector3 size = new Vector3(cellSize, 0.01f, cellSize);

                float t = Mathf.Clamp01(count / 10f);
                Gizmos.color = Color.Lerp(Color.green, Color.red, t);
                Gizmos.DrawCube(center, size);

                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
#endif