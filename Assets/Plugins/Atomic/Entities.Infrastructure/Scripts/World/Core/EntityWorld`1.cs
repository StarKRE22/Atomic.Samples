using System.Collections.Generic;
using Atomic.Infrastructure;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// Represents a runtime-managed world composed of entities of type <typeparamref name="E"/>.
    /// Provides lifecycle management including, enabling, updating, and disposing all entities in the collection.
    /// </summary>
    /// <typeparam name="E">The specific type of entity managed by this world. Must implement <see cref="IEntity"/>.</typeparam>
    public class EntityWorld<E> : World<E> where E : IEntity
    {
        /// <summary>
        /// Initializes a new <see cref="EntityWorld{E}"/> with the specified entities and an empty name.
        /// </summary>
        /// <param name="entities">Entities to prepopulate the world with.</param>
        public EntityWorld(params E[] entities) : base(entities)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EntityWorld{E}"/> with a name and optional entities.
        /// </summary>
        /// <param name="name">The name of the world.</param>
        /// <param name="entities">Entities to prepopulate the world with.</param>
        public EntityWorld(string name = null, params E[] entities) : base(name, entities)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EntityWorld{E}"/> with a name and a collection of entities.
        /// </summary>
        /// <param name="name">The name of the world.</param>
        /// <param name="entities">The enumerable collection of entities to add.</param>
        public EntityWorld(string name, IEnumerable<E> entities) : base(name, entities)
        {
        }

        protected override void Enable(E obj) => obj.Enable();

        protected override void Disable(E obj) => obj.Disable();

        protected override void Tick(E obj, float deltaTime) => obj.Tick(deltaTime);

        protected override void FixedTick(E obj, float deltaTime) => obj.FixedTick(deltaTime);

        protected override void LateTick(E obj, float deltaTime) => obj.LateTick(deltaTime);
    }
}