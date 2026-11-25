using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    public sealed class ProjectileMoveProcessor : IProcessor<IGameEntity>
    {
        private readonly IGameContext _gameContext;

        public ProjectileMoveProcessor(IGameContext context)
        {
            _gameContext = context;
        }

        void IProcessor<IGameEntity>.Process(IGameEntity entity, float deltaTime)
        {
            IGameEntity target = entity.GetTarget().Value;
            if (target is not {Enabled: true})
            {
                entity.MoveStep(entity.GetForwardDirection(), deltaTime);
                return;
            }

            Vector3 vector = entity.GetDistanceVector(target);
            float scale = entity.GetScale().Value;
            if (vector.sqrMagnitude > scale * scale)
            {
                entity.GetRotation().Value = Quaternion.LookRotation(vector.normalized);
                entity.MoveStep(vector.normalized, deltaTime);
            }
            else if (entity.DealDamage(target))
                _gameContext.Despawn(entity);
        }
    }
}