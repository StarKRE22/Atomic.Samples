using System;
using Atomic.Entities;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public sealed class FireUnitsSystem : RoundGameEntitySystem
    {
        public FireUnitsSystem(IGameContext context)
            : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(), entity => entity.HasFireableTag()))
        {
        }

        protected override void Update(IGameEntity entity, float deltaTime)
        {
            if (entity == null)
            {
                Debug.Log("ENTITY NULL");
            }
            
            if (entity.TryGetFireCooldown(out var cooldown))
                cooldown.Tick(deltaTime);

            if (entity.GetFireRequest().Consume(out IGameEntity target))
                entity.GetFireCommand().Invoke(target);
        }
    }
}