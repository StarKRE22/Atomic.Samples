using System;

namespace Atomic.Infrastructure
{
    public interface IManager<T> : IStore<T>
    {
        #region Enable

        /// <summary>
        /// Occurs when the object is enabled.
        /// </summary>
        event Action OnEnabled;

        /// <summary>
        /// Occurs when the object is disabled.
        /// </summary>
        event Action OnDisabled;

        /// <summary>
        /// Gets a value indicating whether the object is currently enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Enables the object.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the object.
        /// </summary>
        void Disable();

        #endregion

        #region Tick

        /// <summary>
        /// Occurs during the regular Update phase, once per frame.
        /// </summary>
        event Action<float> OnTicked;

        /// <summary>
        /// Occurs during the FixedUpdate phase, used for physics updates.
        /// </summary>
        event Action<float> OnFixedTicked;

        /// <summary>
        /// Occurs during the LateUpdate phase, after all Update calls have been made.
        /// </summary>
        event Action<float> OnLateTicked;

        /// <summary>
        /// Called once per frame during the Update phase.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last frame.</param>
        void Tick(float deltaTime);

        /// <summary>
        /// Called during the FixedUpdate phase, typically used for physics calculations.
        /// </summary>
        /// <param name="deltaTime">The fixed time step used by the physics engine.</param>
        void FixedTick(float deltaTime);

        /// <summary>
        /// Called during the LateUpdate phase, after all Update calls.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last frame.</param>
        void LateTick(float deltaTime);

        #endregion
    }
}