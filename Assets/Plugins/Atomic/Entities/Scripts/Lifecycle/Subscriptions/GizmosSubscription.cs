#if UNITY_EDITOR
using System;

namespace Atomic.Entities
{
    public readonly struct GizmosSubscription : IDisposable
    {
        private readonly IGizmosObservable observable;
        private readonly Action _callback;

        public GizmosSubscription(IGizmosObservable observable, Action callback)
        {
            this.observable = observable ?? throw new ArgumentNullException(nameof(observable));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.observable.OnGizmosDraw += _callback;
        }

        public void Dispose()
        {
            observable.OnGizmosDraw -= _callback;
        }
    }
}
#endif