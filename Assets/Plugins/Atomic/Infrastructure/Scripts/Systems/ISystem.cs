using System;

namespace Atomic.Infrastructure
{
    public interface ISystem : IDisposable
    {
        /// <summary>
        /// Enables the system, subscribes to entity collection events,
        /// and registers all existing entities.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the system, unsubscribes from entity collection events,
        /// and unregisters all tracked entities.
        /// </summary>
        void Disable();

        /// <summary>
        /// Updates the system.
        /// The execution time is measured and the processing batch size
        /// is automatically adjusted to fit within the configured frame budget.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the previous update.</param>
        void Update(float deltaTime);
    }
}