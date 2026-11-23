using System;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a disposable subscription handle to an <see cref="ITickLifecycle"/>'s <see cref="ITickLifecycle.OnFixedTicked"/> event.
    /// </summary>
    /// <remarks>
    /// Allows temporarily reacting to fixed update ticks of an <see cref="ITickLifecycle"/> instance.
    /// When disposed, the subscription automatically detaches the callback to prevent memory leaks
    /// or repeated invocations.
    /// 
    /// Subscriptions are intended to be short-lived and can be safely used in a <c>using</c> statement or manually disposed.
    /// </remarks>
    public readonly struct FixedTickSubscription : IDisposable
    {
        private readonly ITickLifecycle lifecycle;
        private readonly Action<float> _callback;

        /// <summary>
        /// Initializes a new <see cref="FixedTickSubscription"/> instance.
        /// Subscribes the specified callback to the <see cref="ITickLifecycle.OnFixedTicked"/> event of the provided source.
        /// </summary>
        /// <param name="lifecycle">The <see cref="ITickLifecycle"/> instance to subscribe to.</param>
        /// <param name="callback">The callback action to invoke on each fixed update tick.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="lifecycle"/> or <paramref name="callback"/> is <c>null</c>.</exception>
        public FixedTickSubscription(ITickLifecycle lifecycle, Action<float> callback)
        {
            this.lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.lifecycle.OnFixedTicked += _callback;
        }

        /// <summary>
        /// Unsubscribes the callback from the <see cref="ITickLifecycle.OnFixedTicked"/> event.
        /// Calling this method ensures the callback will no longer be invoked.
        /// Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            lifecycle.OnFixedTicked -= _callback;
        }
    }
}