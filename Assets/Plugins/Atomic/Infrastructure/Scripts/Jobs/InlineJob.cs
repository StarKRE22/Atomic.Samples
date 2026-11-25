using System;

namespace Atomic.Infrastructure
{
    public sealed class InlineJob<T> : IJob<T>
    {
        private readonly JobHandler<T> _handler;

        public InlineJob(JobHandler<T> handler) =>
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        public void Execute(T obj, float deltaTime) =>
            _handler.Invoke(obj, deltaTime);
    }
}