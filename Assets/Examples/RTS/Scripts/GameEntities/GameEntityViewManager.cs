using Atomic.Entities;

namespace RTSGame
{
    public sealed class GameEntityViewManager : EntityViewManagerSingleton<GameEntityType, IGameEntity, GameEntityView>
    {
        protected override GameEntityType GetKey(IGameEntity entity) => entity.GetEntityType().Value;
    }
}