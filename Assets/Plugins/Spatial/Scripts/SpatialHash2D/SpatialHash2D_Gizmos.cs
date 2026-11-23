#if UNITY_EDITOR
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        private const float GIZMOS_Y_OFFSET = 0.05f;

        public void DrawGizmosXZ()
        {
            Vector3 size = new Vector3(_cellSize, 0.01f, _cellSize);
            
            foreach (long key in _cells.Keys)
            {
                int x = (int) (key >> 32);
                int y = (int) key;
                Vector2Int position = new Vector2Int(x, y);
                int count = this.GetCount(position);

                Vector3 center = new Vector3(
                    (position.x + 0.5f) * _cellSize,
                    GIZMOS_Y_OFFSET,
                    (position.y + 0.5f) * _cellSize
                );
                
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