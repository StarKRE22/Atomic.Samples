using Atomic.Elements;
using Atomic.Infrastructure;

namespace RTSGame
{
    public sealed class DetectTargetJob : IJob<IGameEntity>
    {
        private readonly IGameContext _gameContext;
        private readonly IWorld<IGameEntity> _entityWorld;

        public DetectTargetJob(IGameContext gameContext)
        {
            _gameContext = gameContext;
            _entityWorld = gameContext.GetEntityWorld();
        }

        void IJob<IGameEntity>.Execute(IGameEntity entity, float deltaTime)
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

