using System;
using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    public sealed class UpdateGameEntityPriorityJob : IJob<IGameEntity>
    {
        [Serializable]
        public sealed class Settings
        {
            public float highDistance = 200f;
            public float mediumDistance = 400f;
        }
        
        private readonly Settings _settings;
        private readonly Transform _target;

        public UpdateGameEntityPriorityJob(Settings settings, Transform target)
        {
            _settings = settings;
            _target = target;
        }

        void IJob<IGameEntity>.Execute(IGameEntity entity, float deltaTime)
        {
            Vector3 position = entity.GetPosition().Value;
            float distance = Vector3.Magnitude(position - _target.position);

            entity.GetUpdatePriority().Value = distance <= _settings.highDistance
                ? Priority.High
                : distance <= _settings.mediumDistance
                    ? Priority.Medium
                    : Priority.Low;
        }
    }
}