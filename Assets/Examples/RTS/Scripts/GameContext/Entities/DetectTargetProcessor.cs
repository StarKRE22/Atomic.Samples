using System;
using Atomic.Elements;
using Atomic.Infrastructure;

namespace RTSGame
{
    [Serializable]
    public sealed class DetectTargetProcessor : IProcessor<IGameEntity>
    {
        private readonly IGameContext _gameContext;
        private readonly IWorld<IGameEntity> _entityWorld;

        public DetectTargetProcessor(IGameContext gameContext)
        {
            _gameContext = gameContext;
            _entityWorld = gameContext.GetEntityWorld();
        }

        void IProcessor<IGameEntity>.Process(IGameEntity entity, float deltaTime)
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

