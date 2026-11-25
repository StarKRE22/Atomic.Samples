using Atomic.Elements;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace RTSGame
{
    [BurstCompile]
    public static class MoveUseCase
    {
        public static void ProcessMove(this IGameEntity entity, float deltaTime)
        {
            if (entity.GetMoveRequest().Consume(out Vector3 direction))
                entity.GetMoveCommand().Invoke(direction, deltaTime);
        }
        
        public static void MoveStep(this IGameEntity entity, Vector3 direction, float deltaTime)
        {
            IReactiveVariable<Vector3> position = entity.GetPosition();
            MoveStep(
                position.Value,
                direction,
                entity.GetMoveSpeed().Value,
                deltaTime,
                out float3 next
            );
            position.Value = next;
        }
        
        [BurstCompile]
        public static void MoveStep(
            in float3 position,
            in float3 direction,
            in float speed,
            in float deltaTime,
            out float3 result
        ) => result = position + speed * deltaTime * direction;
    }
}