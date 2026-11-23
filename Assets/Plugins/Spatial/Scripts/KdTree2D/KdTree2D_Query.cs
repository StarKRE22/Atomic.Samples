using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public bool QueryPoint(Vector2 point, out IReadOnlyList<T> values)
        {
            Node node = FindNode(root, point, 0);

            if (node != null)
            {
                values = node.values;
                return true;
            }

            values = null;
            return false;
        }
    }
}