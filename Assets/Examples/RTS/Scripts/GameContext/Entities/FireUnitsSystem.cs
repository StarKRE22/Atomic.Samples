using System;
using Atomic.Entities;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public sealed class FireUnitsSystem : EntityPriorityBudgetGameSystem
    {
        public FireUnitsSystem(IGameContext context)
            : base(new EntityFilter<IGameEntity>(context.GetEntityWorld()))
        {
        }

        protected override void Update(IGameEntity entity, float deltaTime)
        {
            if (entity == null)
            {
                Debug.Log("ENTITY NULL");
            }
            
            
        }
    }
}