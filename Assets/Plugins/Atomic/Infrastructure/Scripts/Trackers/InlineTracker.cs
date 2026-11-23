using System;

namespace Atomic.Infrastructure
{
    public class InlineTracker<T> : ITracker<T>
    {
        private Action<T> _action;

        private readonly Action<T, Action<T>> _track;
        private readonly Action<T, Action<T>> _untrack;

        public InlineTracker(Action<T, Action<T>> track, Action<T, Action<T>> untrack)
        {
            _track = track ?? throw new ArgumentNullException(nameof(track));
            _untrack = untrack ?? throw new ArgumentNullException(nameof(untrack));
        }

        public void SetTrigger(Action<T> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Track(T entity) => _track.Invoke(entity, _action);

        public void Untrack(T entity) => _untrack.Invoke(entity, _action);
    }
}