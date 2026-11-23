using System.Runtime.CompilerServices;

namespace Atomic.Entities
{
    public partial class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindWorld<E>(this IEntity entity, IEntityWorld<E> world) where E : IEntity
        {
            entity.WhenInit(world.InitEntities);
            entity.WhenEnable(world.Enable);
            entity.WhenTick(world.Tick);
            entity.WhenFixedTick(world.FixedTick);
            entity.WhenLateTick(world.LateTick);
            entity.WhenDisable(world.Disable);
            entity.WhenDispose(world.DisposeEntities);
            entity.WhenDispose(world.Dispose);
        }
    }
}