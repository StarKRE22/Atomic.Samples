using Atomic.Elements;
using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    [CreateAssetMenu(
        fileName = "PlayerContextBuilder",
        menuName = "RTSGame/New PlayerContextBuilder"
    )]
    public sealed class PlayerContextFactory : ScriptableEntityFactory<IPlayerContext, Args<TeamType, IGameContext>>
    {
        private const string PLAYER_CONTEXT_NAME_FORMAT = "PlayerContext {0}";

        protected override IPlayerContext Create(
            int tagCapacity,
            int valueCapacity,
            int behaviourCapacity,
            Entity.Settings settings,
            Args<TeamType, IGameContext> args
        )
        {
            TeamType team = args.value1;
            IGameContext gameContext = args.value2;

            string playerName = string.Format(PLAYER_CONTEXT_NAME_FORMAT, team);
            PlayerContext playerContext = new PlayerContext(
                playerName,
                tagCapacity,
                valueCapacity,
                behaviourCapacity
            );

            IReadOnlyStore<IGameEntity> entityManager = gameContext.GetEntityWorld();
            Filter<IGameEntity> enemies = new Filter<IGameEntity>(entityManager,
                e => e.HasUnitTag() && team != e.GetTeam().Value);
            playerContext.AddEnemies(enemies);
            playerContext.AddTeam(new Const<TeamType>(team));
            return playerContext;
        }
    }
}