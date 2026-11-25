using Game.Gameplay;

namespace Game.UI
{
    public static class GameEntityViewUseCase
    {
        public static GameEntityView GetEntityView(this IUIContext ui, IGameEntity entity)
        {
            GameEntityViewStore viewStore = ui.GetEntityCollectionView();
            return viewStore.Get(entity);
        }
    }
}