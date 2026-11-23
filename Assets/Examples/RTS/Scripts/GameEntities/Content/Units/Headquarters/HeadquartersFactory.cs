using Atomic.Elements;
using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    [CreateAssetMenu(
        fileName = "HeadquartersFactory",
        menuName = "RTSGame/GameEntities/New HeadquartersFactory"
    )]
    public sealed class HeadquartersFactory : GameEntityFactory
    {
        [SerializeField]
        private TransformEntityInstaller _transformInstaller;
        
        [SerializeField]
        private LifeEntityInstaller _lifeInstaller;

        protected override void Install(IGameEntity entity, IGameContext gameContext)
        {
            entity.AddUnitTag();
            entity.AddTeam(new ReactiveVariable<TeamType>());
            entity.AddBehaviour(new EntitySpaceBehaviour(gameContext));
            entity.AddUpdatePriority(new ReactiveVariable<UpdatePriority>(UpdatePriority.High));
            
            _transformInstaller.Install(entity);
            _lifeInstaller.Install(entity, gameContext);
        }
    }
}

