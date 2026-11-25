using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    public sealed class GamePresenter : MonoBehaviour
    {
        [SerializeField]
        private GameEntityViewWorld _worldView;

        [SerializeField]
        private GameEntityViewPool _viewPool;
        
        private void Start()
        {
            _ = _viewPool.InitAsync(GameEntityType.Projectile, 4096);
            IGameContext gameContext = FindAnyObjectByType<GameRunner>().GameContext;
            IWorld<IGameEntity> entities = gameContext.GetEntityWorld();
            _worldView.Show(entities);
        }
    }
}