using System.Collections.Generic;

namespace Atomic.Entities
{
    /// <summary>
    /// A non-generic shortcut for <see cref="EntityManager{E}"/>.
    /// Manages a world of general-purpose <see cref="IEntity"/> instances with support for enabling,
    /// updating, and disposing all contained entities.
    /// </summary>
    /// <remarks>
    /// This class provides a convenient entry point for working with untyped or heterogeneous entities
    /// without requiring generic parameters.
    /// </remarks>
    public class EntityManager : EntityManager<IEntity>
    {
        /// <summary>
        /// Initializes an empty <see cref="EntityManager"/> instance with no name.
        /// </summary>
        public EntityManager()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EntityManager"/> with the specified entities and an empty name.
        /// </summary>
        /// <param name="objects">Entities to prepopulate the world with.</param>
        public EntityManager(params IEntity[] objects) : base(objects)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EntityManager"/> with the specified entities and an empty name.
        /// </summary>
        /// <param name="objects">Entities to prepopulate the world with.</param>
        public EntityManager(string name = null, params IEntity[] objects) : base(name, objects)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EntityManager"/> with a name and a collection of entities.
        /// </summary>
        /// <param name="name">The name of the world.</param>
        /// <param name="objects">The enumerable collection of entities to add.</param>
        public EntityManager(string name, IEnumerable<IEntity> objects) : base(name, objects)
        {
        }
    }
}