using System.Runtime.CompilerServices;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public partial class Extensions
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static void BindWorld<E>(this IEntity entity, IWorld<E> world) where E : IEntity
        // {
        //     entity.WhenInit(world.InitEntities);
        //     entity.WhenEnable(world.Enable);
        //     entity.WhenTick(world.Tick);
        //     entity.WhenFixedTick(world.FixedTick);
        //     entity.WhenLateTick(world.LateTick);
        //     entity.WhenDisable(world.Disable);
        //     entity.WhenDispose(world.DisposeEntities);
        //     entity.WhenDispose(world.Dispose);
        // }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WorldBinder<E> BindWorld<E>(this IEntity entity, IWorld<E> world) where E : IEntity
        {
            entity.WhenInit(world.InitEntities);
            entity.WhenEnable(world.Enable);
            entity.WhenDisable(world.Disable);
            entity.WhenDispose(world.DisposeEntities);
            entity.WhenDispose(world.Dispose);

            return new WorldBinder<E>(entity, world);
        }

        public readonly struct WorldBinder<E> where E : IEntity
        {
            private readonly IEntity entity;
            private readonly IWorld<E> world;

            public WorldBinder(IEntity entity, IWorld<E> world)
            {
                this.entity = entity;
                this.world = world;
            }

            public WorldBinder<E> EveryTick()
            {
                this.entity.WhenTick(this.world.Tick);
                return this;
            }

            public WorldBinder<E> EveryFixedTick()
            {
                this.entity.WhenFixedTick(this.world.FixedTick);
                return this;
            }

            public WorldBinder<E> EveryLateTick()
            {
                this.entity.WhenLateTick(this.world.LateTick);
                return this;
            }

            public void EveryUpdate()
            {
                this.entity.WhenTick(this.world.Tick);
                this.entity.WhenFixedTick(this.world.FixedTick);
                this.entity.WhenLateTick(this.world.LateTick);
            }
        }
    }
}