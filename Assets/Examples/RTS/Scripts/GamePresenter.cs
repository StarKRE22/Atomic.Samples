using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    public sealed class GamePresenter : MonoBehaviour
    {
        [SerializeField]
        private GameEntityViewManager managerView;

        [SerializeField]
        private GameEntityViewPool _viewPool;
        
        private void Start()
        {
            _ = _viewPool.InitAsync(GameEntityType.Projectile, 4096);
            IGameContext gameContext = FindAnyObjectByType<GameRunner>().GameContext;
            IReadOnlyStore<IGameEntity> entities = gameContext.GetEntityWorld();
            managerView.Show(entities);
        }
    }
}