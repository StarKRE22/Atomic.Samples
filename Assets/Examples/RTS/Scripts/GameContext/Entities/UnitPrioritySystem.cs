using System;
using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    public sealed class UnitPrioritySystem : EntityBudgetGameSystem
    {
        [Serializable]
        public sealed class Settings
        {
            public float highDistance = 200f;
            public float mediumDistance = 400f;
        }

        private readonly Settings _settings;
        private Transform _target;

        public UnitPrioritySystem(
            IGameContext context,
            Settings settings
        ) : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(), e => e.HasUnitTag()))
        {
            _settings = settings;
        }

        protected override void Update(IGameEntity entity, float deltaTime)
        {
            if (_target == null)
                _target = Camera.main?.transform;

            if (_target == null)
                return;

            Vector3 entityPosition = entity.GetPosition().Value;
            float distance = Vector3.Magnitude(entityPosition - _target.position);

            entity.GetUpdatePriority().Value = distance <= _settings.highDistance
                ? UpdatePriority.High
                : distance <= _settings.mediumDistance
                    ? UpdatePriority.Medium
                    : UpdatePriority.Low;
        }
    }
}