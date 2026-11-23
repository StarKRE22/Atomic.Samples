using UnityEngine;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public bool Move(T value, Vector2 p)
        {
            if (!this.Remove(value))
                return false;

            this.Add(value, p);
            return true;
        }
    }
}