using Atomic.Entities;

namespace Game.UI
{
    public static class DespawnUseCase
    {
        public static void Despawn(this GameEntityView entityView, IUIContext uiContext)
        {
            GameEntityStoreView storeView = uiContext.GetEntityCollectionView();
            storeView.Remove(entityView.Model);
        }
    }
}