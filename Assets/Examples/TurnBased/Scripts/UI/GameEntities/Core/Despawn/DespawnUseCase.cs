using Atomic.Entities;

namespace Game.UI
{
    public static class DespawnUseCase
    {
        public static void Despawn(this GameEntityView entityView, IUIContext uiContext)
        {
            GameEntityViewStore viewStore = uiContext.GetEntityCollectionView();
            viewStore.Remove(entityView.Model);
        }
    }
}