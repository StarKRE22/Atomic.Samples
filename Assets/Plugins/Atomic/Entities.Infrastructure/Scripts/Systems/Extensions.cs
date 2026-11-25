using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public static partial class Extensions
    {
        public static SystemBinder BindSystem(this IEntity entity, ISystem system)
        {
            entity.WhenEnable(system.Enable);
            entity.WhenDisable(system.Disable);
            entity.WhenDispose(system.Dispose);
            return new SystemBinder(entity, system);
        }

        public readonly struct SystemBinder
        {
            private readonly IEntity entity;
            private readonly ISystem system;

            public SystemBinder(IEntity entity, ISystem system)
            {
                this.entity = entity;
                this.system = system;
            }

            public void EveryTick() => this.entity.WhenTick(this.system.Update);

            public void EveryFixedTick() => this.entity.WhenFixedTick(this.system.Update);

            public void EveryLateTick() => this.entity.WhenLateTick(this.system.Update);
        }

        /// <summary>
        /// Registers an entity system that is updated during the Tick phase of the context entity lifecycle.
        /// </summary>
        /// <typeparam name="E">Type of the context entity.</typeparam>
        /// <typeparam name="TEntity">Type of entities processed by the system.</typeparam>
        /// <param name="entity">The context entity.</param>
        /// <param name="system">The entity system to register.</param>    
        public static void BindTickSystem(this IEntity entity, ISystem system)
        {
            entity.WhenEnable(system.Enable);
            entity.WhenTick(system.Update);
            entity.WhenDisable(system.Disable);
            entity.WhenDispose(system.Dispose);
        }


        /// <summary>
        /// Registers an entity system that is updated during the FixedTick phase of the context entity lifecycle.
        /// </summary>
        /// <typeparam name="E">Type of the context entity.</typeparam>
        /// <typeparam name="TEntity">Type of entities processed by the system.</typeparam>
        /// <param name="entity">The context entity.</param>
        /// <param name="system">The entity system to register.</param>
        public static void BindFixedTickSystem(this IEntity entity, ISystem system)
        {
            entity.WhenEnable(system.Enable);
            entity.WhenFixedTick(system.Update);
            entity.WhenDisable(system.Disable);
            entity.WhenDispose(system.Dispose);
        }

        /// <summary>
        /// Registers an entity system that is updated during the LateTick phase of the context entity lifecycle.
        /// </summary>
        /// <typeparam name="E">Type of the context entity.</typeparam>
        /// <typeparam name="TEntity">Type of entities processed by the system.</typeparam>
        /// <param name="entity">The context entity.</param>
        /// <param name="system">The entity system to register.</param>
        public static void BindLateTickSystem(this IEntity entity, ISystem system)
        {
            entity.WhenEnable(system.Enable);
            entity.WhenLateTick(system.Update);
            entity.WhenDisable(system.Disable);
            entity.WhenDispose(system.Dispose);
        }
    }
}