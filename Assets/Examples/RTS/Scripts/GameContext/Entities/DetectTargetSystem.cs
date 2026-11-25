using System;
using Atomic.Elements;
using Atomic.Entities;

namespace RTSGame
{
    [Serializable]
    public sealed class DetectTargetSystem : RoundGameEntitySystem
    {
        private readonly IEntityWorld<IGameEntity> _entityWorld;
        private readonly IGameContext _gameContext;

        public DetectTargetSystem(IGameContext context, int budget = 4096) : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(),
            e => e.HasDetectorTag()), budget)
        {
            _gameContext = context;
            _entityWorld = context.GetEntityWorld();
        }

        protected override void Update(IGameEntity entity, float deltaTime)
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