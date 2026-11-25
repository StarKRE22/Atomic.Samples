using System.Runtime.CompilerServices;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public partial class Extensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ManagerBinder<E> BindManager<E>(this IEntity entity, IManager<E> manager) where E : IEntity
        {
            entity.WhenInit(manager.InitEntities);
            entity.WhenEnable(manager.Enable);
            entity.WhenDisable(manager.Disable);
            entity.WhenDispose(manager.DisposeEntities);
            entity.WhenDispose(manager.Dispose);

            return new ManagerBinder<E>(entity, manager);
        }

        public readonly struct ManagerBinder<E> where E : IEntity
        {
            private readonly IEntity entity;
            private readonly IManager<E> manager;

            public ManagerBinder(IEntity entity, IManager<E> manager)
            {
                this.entity = entity;
                this.manager = manager;
            }

            public ManagerBinder<E> EveryTick()
            {
                this.entity.WhenTick(this.manager.Tick);
                return this;
            }

            public ManagerBinder<E> EveryFixedTick()
            {
                this.entity.WhenFixedTick(this.manager.FixedTick);
                return this;
            }

            public ManagerBinder<E> EveryLateTick()
            {
                this.entity.WhenLateTick(this.manager.LateTick);
                return this;
            }

            public void EveryUpdate()
            {
                this.entity.WhenTick(this.manager.Tick);
                this.entity.WhenFixedTick(this.manager.FixedTick);
                this.entity.WhenLateTick(this.manager.LateTick);
            }
        }
    }
}