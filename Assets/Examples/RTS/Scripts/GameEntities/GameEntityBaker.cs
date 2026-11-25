using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    [RequireComponent(typeof(GameEntityView))]
    public abstract class GameEntityBaker :
        EntityBakerDefault<GameEntityType, IGameEntity, GameEntityView, Args<IGameContext>>
    {
    }
}