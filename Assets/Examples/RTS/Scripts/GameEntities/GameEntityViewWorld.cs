using Atomic.Entities;

namespace RTSGame
{
    public sealed class GameEntityViewWorld : EntityViewWorldSingleton<GameEntityType, IGameEntity, GameEntityView>
    {
        protected override GameEntityType GetKey(IGameEntity entity) => entity.GetEntityType().Value;
    }
}