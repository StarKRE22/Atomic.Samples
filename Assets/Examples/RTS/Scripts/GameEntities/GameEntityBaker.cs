using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    [RequireComponent(typeof(GameEntityView))]
    public abstract class GameEntityBaker : MonoEntityBakerOptimized<GameEntityType, IGameEntity, GameEntityView, Args<IGameContext>>
    {
    }
}