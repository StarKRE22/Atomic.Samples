using System;

namespace Atomic.Infrastructure
{
    public sealed class InlinePriorityStrategy<T> : IPriorityStrategy<T>
    {
        private readonly Func<T, Priority> _func;

        public InlinePriorityStrategy(Func<T, Priority> func) =>
            _func = func ?? throw new ArgumentNullException(nameof(func));

        public Priority Evaluate(T obj) => _func.Invoke(obj);
    }
}