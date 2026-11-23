using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a world that manages a collection of entities and controls their lifecycle events.
    /// </summary>
    /// <typeparam name="E">The type of entity managed by this world. Must implement <see cref="IEntity"/>.</typeparam>
    public interface IEntityWorld<E> : IWorld<E>, IEntityCollection<E>, IEnableLifecycle, ITickLifecycle where E : IEntity
    {
        #region Enable

        /// <summary>
        /// Occurs when the object is enabled.
        /// </summary>
        new event Action OnEnabled;

        /// <summary>
        /// Occurs when the object is disabled.
        /// </summary>
        new event Action OnDisabled;

        /// <summary>
        /// Gets a value indicating whether the object is currently enabled.
        /// </summary>
        new bool Enabled { get; }

        /// <summary>
        /// Enables the object.
        /// </summary>
        new void Enable();

        /// <summary>
        /// Disables the object.
        /// </summary>
        new void Disable();

        #endregion

        #region Tick

        /// <summary>
        /// Occurs during the regular Update phase, once per frame.
        /// </summary>
        new event Action<float> OnTicked;

        /// <summary>
        /// Occurs during the FixedUpdate phase, used for physics updates.
        /// </summary>
        new event Action<float> OnFixedTicked;

        /// <summary>
        /// Occurs during the LateUpdate phase, after all Update calls have been made.
        /// </summary>
        new event Action<float> OnLateTicked;

        /// <summary>
        /// Called once per frame during the Update phase.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last frame.</param>
        new void Tick(float deltaTime);

        /// <summary>
        /// Called during the FixedUpdate phase, typically used for physics calculations.
        /// </summary>
        /// <param name="deltaTime">The fixed time step used by the physics engine.</param>
        new void FixedTick(float deltaTime);

        /// <summary>
        /// Called during the LateUpdate phase, after all Update calls.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last frame.</param>
        new void LateTick(float deltaTime);

        #endregion
    }
}