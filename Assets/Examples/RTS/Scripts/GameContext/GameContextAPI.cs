using Atomic.Entities;
using Modules.Spatial;
using System.Collections.Generic;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    [EntityExtensionsAPI(Unsafe = true)]
    public static partial class GameContextAPI
    {
        public static readonly ValueKey<IGameContext, IWorld<IGameEntity>> EntityWorld = new(nameof(EntityWorld));
        public static readonly ValueKey<IGameContext, IMultiPool<GameEntityType, IGameEntity>> EntityPool = new(nameof(EntityPool));
        public static readonly ValueKey<IGameContext, Dictionary<TeamType, IPlayerContext>> Players = new(nameof(Players));
        public static readonly ValueKey<IGameContext, TeamViewConfig> TeamViewConfig = new(nameof(TeamViewConfig));
        public static readonly ValueKey<IGameContext, Transform> PlayerPoint = new(nameof(PlayerPoint));
        public static readonly ValueKey<IGameContext, ISpatialDataStructure2D<IGameEntity>> EntitySpace = new(nameof(EntitySpace));
    }
}
