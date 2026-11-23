using System;
using System.Runtime.CompilerServices;

namespace Atomic.Entities
{
    /// <summary>
    /// Provides extension methods for event-based subscriptions to lifecycle interfaces.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Subscribes to the <see cref="IInitLifecycle.OnInitialized"/> event.
        /// </summary>
        /// <param name="lifecycle">The spawnable object.</param>
        /// <param name="action">The action to invoke on spawn.</param>
        /// <returns>A disposable <see cref="InitSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InitSubscription WhenInit(this IInitLifecycle lifecycle, Action action) =>
            new(lifecycle, action);

        /// <summary>
        /// Subscribes to the <see cref="IInitLifecycle.OnDisposed"/> event.
        /// </summary>
        /// <param name="lifecycle">The spawnable object.</param>
        /// <param name="action">The action to invoke on despawn.</param>
        /// <returns>A disposable <see cref="DisposeSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposeSubscription WhenDispose(this IInitLifecycle lifecycle, Action action) =>
            new(lifecycle, action);

        /// <summary>
        /// Subscribes to the <see cref="IEnableLifecycle.OnEnabled"/> event.
        /// </summary>
        /// <param name="lifecycle">The activatable object.</param>
        /// <param name="action">The action to invoke when enabled.</param>
        /// <returns>A disposable <see cref="EnableSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnableSubscription WhenEnable(this IEnableLifecycle lifecycle, Action action) =>
            new(lifecycle, action);

        /// <summary>
        /// Subscribes to the <see cref="IEnableLifecycle.OnDisables"/> event.
        /// </summary>
        /// <param name="lifecycle">The activatable object.</param>
        /// <param name="action">The action to invoke when disabled.</param>
        /// <returns>A disposable <see cref="DisableSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisableSubscription WhenDisable(this IEnableLifecycle lifecycle, Action action) =>
            new(lifecycle, action);

        /// <summary>
        /// Subscribes to the <see cref="ITickLifecycle.OnTicked"/> event.
        /// </summary>
        /// <param name="lifecycle">The updatable object.</param>
        /// <param name="action">The action to invoke each frame update.</param>
        /// <returns>A disposable <see cref="TickSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TickSubscription WhenTick(this ITickLifecycle lifecycle, Action<float> action) =>
            new(lifecycle, action);

        /// <summary>
        /// Subscribes to the <see cref="ITickLifecycle.OnFixedTicked"/> event.
        /// </summary>
        /// <param name="lifecycle">The updatable object.</param>
        /// <param name="action">The action to invoke each fixed update frame.</param>
        /// <returns>A disposable <see cref="FixedTickSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedTickSubscription WhenFixedTick(this ITickLifecycle lifecycle, Action<float> action) =>
            new(lifecycle, action);

        /// <summary>
        /// Subscribes to the <see cref="ITickLifecycle.OnLateTicked"/> event.
        /// </summary>
        /// <param name="lifecycle">The updatable object.</param>
        /// <param name="action">The action to invoke each late update frame.</param>
        /// <returns>A disposable <see cref="LateTickSubscription"/> that unsubscribes when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LateTickSubscription WhenLateTick(this ITickLifecycle lifecycle, Action<float> action) =>
            new(lifecycle, action);

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WhenGizmos(this IGizmosObservable observable, Action action) =>
            observable.OnGizmosDraw += action;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddGizmosBehaviour(this IEntity source, Action action) =>
            source.AddBehaviour(new GizmosBehaviour(action));

        private sealed class GizmosBehaviour : IEntityGizmos
        {
            private readonly Action _action;

            public GizmosBehaviour(Action action) => _action = action;

            void IEntityGizmos.DrawGizmos(IEntity entity) => _action.Invoke();
        }
#endif
    }
}