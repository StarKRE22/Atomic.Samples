using System;

namespace Atomic.Infrastructure
{
    public sealed class InlineProcessor<T> : IProcessor<T>
    {
        private readonly ProcessHandler<T> _handler;

        public InlineProcessor(ProcessHandler<T> handler) =>
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        public void Process(T obj, float deltaTime) =>
            _handler.Invoke(obj, deltaTime);
    }
}