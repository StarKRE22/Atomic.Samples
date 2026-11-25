using System;
using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace Game.Gameplay
{
    [Serializable]
    public sealed class EntitySystemInstaller : IGameContextInstaller
    {
        [SerializeField]
        private GameEntityCatalog _entityCatalog;

        [SerializeField]
        private EntitySpawnInfo[] _initialEntities;

        public void Install(IGameContext context)
        {
            context.AddInitialEntities(_initialEntities);

            var entityManager = new EntityManager<IGameEntity>();
            context.AddEntityWorld(entityManager);
            context.AddEntityPool(new MultiPool<GameEntityType, IGameEntity, Args<IGameContext>>(
                _entityCatalog, new Args<IGameContext>(context)
            ));

            context.WhenInit(context.SpawnInitialUnits);
            context.BindManager(entityManager);
        }
    }
}