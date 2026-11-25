using System;
using Atomic.Entities;

namespace RTSGame
{
    [Serializable]
    public sealed class AttackTargetSystem : EntityPriorityBudgetGameSystem
    {
        public AttackTargetSystem(IGameContext context)
            : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(), entity => entity.HasAttackerTag()))
        {
        }

        protected override void Update(IGameEntity entity, float deltaTime) =>
            entity.Attack();
    }
}