using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public bool Contains(Vector2 point) =>
            this.FindNode(root, point, 0) != null;
    }
}