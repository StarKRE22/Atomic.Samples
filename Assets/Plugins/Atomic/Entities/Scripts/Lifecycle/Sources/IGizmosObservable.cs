#if UNITY_EDITOR
using System;

namespace Atomic.Entities
{
    public interface IGizmosObservable
    {
        event Action OnGizmosDraw;
    }
}
#endif