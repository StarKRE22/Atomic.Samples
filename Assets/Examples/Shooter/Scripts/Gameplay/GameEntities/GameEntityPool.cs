using Atomic.Entities;
using Atomic.Infrastructure;

/**
 * Created by Entity Domain Generator.
 */

namespace ShooterGame.Gameplay
{
    /// <summary>
    /// A Unity-integrated pool for <see cref="GameEntity"/> entities that exist within a scene.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IEntityPool{IGameEntity}"/> for renting and returning scene-based entities.
    /// </remarks>
    public sealed class GameEntityPool : MonoEntityPool<IGameEntity, GameEntity>, IEntityPool<IGameEntity>
    {
        IGameEntity IPool<IGameEntity>.Rent() => this.Rent();

        void IPool<IGameEntity>.Return(IGameEntity entity) => this.Return((GameEntity)entity);
    }
}
