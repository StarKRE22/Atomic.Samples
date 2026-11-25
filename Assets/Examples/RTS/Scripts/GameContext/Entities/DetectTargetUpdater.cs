using System;
using Atomic.Elements;
using Atomic.Entities;
using Atomic.Infrastructure;

namespace RTSGame
{
    [Serializable]
    public sealed class DetectTargetUpdater : IUpdater<IGameEntity>
    {
        private readonly IGameContext _gameContext;
        private readonly IWorld<IGameEntity> _entityWorld;

        public DetectTargetUpdater(IGameContext gameContext)
        {
            _gameContext = gameContext;
            _entityWorld = gameContext.GetEntityWorld();
        }

        void IUpdater<IGameEntity>.Update(IGameEntity entity, float deltaTime)
        {
            ICooldown cooldown = entity.GetDetectionCooldown();
            cooldown.Tick(deltaTime);

            var target = entity.GetTarget();
            if (cooldown.IsCompleted() && !_entityWorld.Contains(target.Value))
            {
                target.Value = _gameContext.FindClosestEnemy(entity);
                cooldown.ResetTime();
            }
        }
    }
}

