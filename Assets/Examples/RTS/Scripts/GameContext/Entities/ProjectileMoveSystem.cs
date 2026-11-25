using System;
using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public sealed class ProjectileMoveSystem : EntityBudgetGameSystem
    {
        private readonly IGameContext _gameContext;

        public ProjectileMoveSystem(IGameContext context)
            : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(), e => e.HasProjectileTag()))
        {
            _gameContext = context;
        }

        protected override void Update(IGameEntity entity, float deltaTime)
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