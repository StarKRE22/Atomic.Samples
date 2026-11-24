using System.Collections.Generic;
using UnityEngine;
// ReSharper disable UnusedMemberInSuper.Global

namespace Modules.Spatial
{
    public interface ISpatialDataStructure2D<T> : IEnumerable<KeyValuePair<T, Vector2>>
    {
        int Count { get; }
        
        bool Add(T item, Vector2 position);
        bool Remove(T item);
        bool Move(T item, Vector2 p);
        bool Contains(T item);
        void Clear();
        
        bool QueryClosest(Vector2 position, out Vector2 closestPoint, out T closest);
        int QueryCircle(Vector2 center, float radius, T[] results);
        int QueryRect(Rect rect, T[] results);
    }
}