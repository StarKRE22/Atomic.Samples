using Atomic.Elements;
using Atomic.Entities;
using Atomic.Infrastructure;

namespace RTSGame
{
    [EntityExtensionsAPI(Unsafe = true)]
    public static partial class PlayerContextAPI
    {
        public static readonly ValueKey<IPlayerContext, IValue<TeamType>> Team = new(nameof(Team));
        public static readonly ValueKey<IPlayerContext, Filter<IGameEntity>> Enemies = new(nameof(Enemies));
    }
}
