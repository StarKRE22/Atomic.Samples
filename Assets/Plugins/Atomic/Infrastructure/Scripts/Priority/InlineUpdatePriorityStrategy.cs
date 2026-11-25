using System;

namespace Atomic.Infrastructure
{
    public sealed class InlineUpdatePriorityStrategy<T> : IUpdatePriorityStrategy<T>
    {
        private readonly Func<T, UpdatePriority> _func;

        public InlineUpdatePriorityStrategy(Func<T, UpdatePriority> func) =>
            _func = func ?? throw new ArgumentNullException(nameof(func));

        public UpdatePriority EvaluatePriority(T obj) => _func.Invoke(obj);
    }
}