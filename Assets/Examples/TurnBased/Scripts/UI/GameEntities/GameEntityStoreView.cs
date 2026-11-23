using Atomic.Entities;
using Game.Gameplay;

namespace Game.UI
{
    public sealed class GameEntityStoreView : EntityViewStore<GameEntityType, IGameEntity, GameEntityView>
    {
        protected override GameEntityType GetKey(IGameEntity entity) => 
            entity.GetEntityType().Value;
    }
}