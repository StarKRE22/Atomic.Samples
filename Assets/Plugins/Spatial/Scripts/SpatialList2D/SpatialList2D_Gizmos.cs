using UnityEditor;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public void DrawGizmosXZ()
        {
            Handles.color = Color.red;
            
            foreach ((T key, Vector2 position) in _entries)
            {
                if (key == null)
                    continue;

                Handles.DrawWireDisc(position, Vector3.up, 0.2f, 1);
            }
        }
    }
}