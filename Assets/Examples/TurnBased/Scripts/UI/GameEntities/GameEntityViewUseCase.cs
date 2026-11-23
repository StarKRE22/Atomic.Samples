using Atomic.Entities;
using Game.Gameplay;

namespace Game.UI
{
    public static class GameEntityViewUseCase
    {
        public static GameEntityView GetEntityView(this IUIContext ui, IGameEntity entity)
        {
            GameEntityStoreView storeView = ui.GetEntityCollectionView();
            return storeView.Get(entity);
        }
    }
}