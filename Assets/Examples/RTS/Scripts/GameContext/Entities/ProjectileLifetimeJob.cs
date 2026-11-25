using Atomic.Elements;
using Atomic.Infrastructure;

namespace RTSGame
{
    public sealed class ProjectileLifetimeJob : IJob<IGameEntity>
    {
        private readonly IGameContext _gameContext;

        public ProjectileLifetimeJob(IGameContext context)
        {
            _gameContext = context;
        }

        void IJob<IGameEntity>.Execute(IGameEntity entity, float deltaTime)
        {
            Cooldown lifetime = entity.GetLifetime();
            lifetime.Tick(deltaTime);
            if (lifetime.IsCompleted())
                _gameContext.Despawn(entity);
        }
    }
}