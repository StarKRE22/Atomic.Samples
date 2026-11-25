using System;
using Atomic.Entities;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public sealed class MoveUnitSystem : EntityPriorityBudgetGameSystem
    {
        public MoveUnitSystem(IGameContext context)
            : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(), e => e.HasUnitTag() && e.HasMoveableTag()))
        {
        }

        protected override void Update(IGameEntity entity, float deltaTime)
        {
            if (entity.GetMoveRequest().Consume(out Vector3 direction))
                entity.GetMoveCommand().Invoke(direction, deltaTime);
        }
    }
}