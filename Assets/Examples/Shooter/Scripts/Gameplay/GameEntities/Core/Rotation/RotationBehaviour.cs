using Atomic.Elements;
using Unity.Mathematics;
using UnityEngine;

namespace ShooterGame.Gameplay
{
    public sealed class RotationBehaviour : IGameEntityInit, IGameEntityFixedTick
    {
        private readonly IRotationUseCase _rotationUseCase;
        
        private IVariable<Quaternion> _rotation;
        private IValue<float> _rotationSpeed;
        private IValue<Vector3> _rotationDirection;
        private IFunction<bool> _rotationCondition;

        public RotationBehaviour(IRotationUseCase rotationUseCase)
        {
            _rotationUseCase = rotationUseCase;
        }

        public void Init(IGameEntity entity)
        {
            _rotation = entity.GetRotation();
            _rotationSpeed = entity.GetRotationSpeed();
            _rotationDirection = entity.GetRotationDirection();
            _rotationCondition = entity.GetRotationCondition();
        }

        public void FixedTick(IGameEntity entity, float deltaTime)
        {
            if (!_rotationCondition.Invoke())
                return;
            
            _rotationUseCase.RotationStep(
                _rotation.Value,
                _rotationDirection.Value,
                _rotationSpeed.Value,
                deltaTime,
                out quaternion next
            );
            
            _rotation.Value = next;
        }
    }
}