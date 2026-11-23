using System;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a disposable subscription handle to an <see cref="IEnableLifecycle"/>'s <see cref=IEnableLifecyclee.OnEnabled"/> event.
    /// </summary>
    /// <remarks>
    /// This struct allows temporarily reacting to enable events of an <see cref="IEnableLifecycle"/> instance.
    /// When the subscription is disposed, it automatically unsubscribes the callback, preventing memory leaks 
    /// or unintended repeated invocations.
    /// 
    /// Subscriptions are intended to be short-lived and can be safely used in a <c>using</c> statement or manually disposed.
    /// </remarks>
    public readonly struct EnableSubscription : IDisposable
    {
        private readonly IEnableLifecycle lifecycle;
        private readonly Action _callback;

        /// <summary>
        /// Initializes a new <see cref="EnableSubscription"/> instance.
        /// Subscribes the specified callback to the <see cref=IEnableLifecyclee.OnEnabled"/> event of the provided source.
        /// </summary>
        /// <param name="lifecycle">The <see cref="IEnableLifecycle"/> instance to subscribe to.</param>
        /// <param name="callback">The callback action to invoke when the source is enabled.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="lifecycle"/> or <paramref name="callback"/> is <c>null</c>.</exception>
        public EnableSubscription(IEnableLifecycle lifecycle, Action callback)
        {
            this.lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.lifecycle.OnEnabled += _callback;
        }

        /// <summary>
        /// Unsubscribes the callback from the <see cref=IEnableLifecyclee.OnEnabled"/> event.
        /// Calling this method ensures the callback will no longer be invoked.
        /// Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            lifecycle.OnEnabled -= _callback;
        }
    }
}