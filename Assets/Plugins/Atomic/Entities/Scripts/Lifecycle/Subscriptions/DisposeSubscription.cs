using System;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a disposable subscription handle to an <see cref="IInitLifecycle"/>'s <see cref="IInitLifecycle.OnDisposed"/> event.
    /// </summary>
    /// <remarks>
    /// This struct allows temporarily reacting to disposal events of an <see cref="IInitLifecycle"/> instance.
    /// When the subscription is disposed, it automatically unsubscribes the callback, preventing memory leaks 
    /// or unintended repeated invocations.
    /// 
    /// Subscriptions are intended to be short-lived and can be safely used in a <c>using</c> statement or manually disposed.
    /// </remarks>
    public readonly struct DisposeSubscription : IDisposable
    {
        private readonly IInitLifecycle lifecycle;
        private readonly Action _callback;

        /// <summary>
        /// Initializes a new <see cref="DisposeSubscription"/> instance.
        /// Subscribes the specified callback to the <see cref="IInitLifecycle.OnDisposed"/> event of the provided source.
        /// </summary>
        /// <param name="lifecycle">The <see cref="IInitLifecycle"/> instance to subscribe to.</param>
        /// <param name="callback">The callback action to invoke when the source is disposed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="lifecycle"/> or <paramref name="callback"/> is <c>null</c>.</exception>
        public DisposeSubscription(IInitLifecycle lifecycle, Action callback)
        {
            this.lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.lifecycle.OnDisposed += _callback;
        }

        /// <summary>
        /// Unsubscribes the callback from the <see cref="IInitLifecycle.OnDisposed"/> event.
        /// Calling this method ensures the callback will no longer be invoked.
        /// Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            lifecycle.OnDisposed -= _callback;
        }
    }
}