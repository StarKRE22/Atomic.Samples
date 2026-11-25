using UnityEngine;

namespace RTSGame
{
    public static class CombatUseCase
    {
        public static void ProcessFire(this IGameEntity entity, float deltaTime)
        {
            if (entity.TryGetFireCooldown(out var cooldown))
                cooldown.Tick(deltaTime);

            if (entity.GetFireRequest().Consume(out IGameEntity target))
                entity.GetFireCommand().Invoke(target);
        }
        
        public static IGameEntity FireProjectile(
            this IGameEntity source,
            IGameEntity target,
            IGameContext context
        )
        {
            IGameEntity projectile = FireProjectile(source, context);
            projectile.GetTarget().Value = target;
            return projectile;
        }

        public static IGameEntity FireProjectile(IGameEntity source, IGameContext context) =>
            context.Spawn(
                GameEntityType.Projectile,
                source.GetFirePoint().Value,
                source.GetRotation().Value,
                source.GetTeam().Value
            );

        public static Vector3 GetFirePoint(this IGameEntity entity, Vector3 offset) =>
            entity.GetPosition().Value + entity.GetRotation().Value * offset;
    }
}