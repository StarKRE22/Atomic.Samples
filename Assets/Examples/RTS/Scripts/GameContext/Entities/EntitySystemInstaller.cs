using System;
using Atomic.Entities;
using Atomic.Infrastructure;
using Modules.Spatial;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public sealed class EntitySystemInstaller : IEntityInstaller<IGameContext>
    {
        [SerializeField]
        private GameEntityCatalog _entityCatalog;

        [SerializeField]
        private UpdateGameEntityPriorityJob.Settings _unitPrioritySettings;

        [SerializeField]
        private ScriptableBudgetStrategy _budgetStrategy;

        public void Install(IGameContext context)
        {
            EntityManager<IGameEntity> entityManager = new EntityManager<IGameEntity>();
            context.AddEntityPool(new MultiPool<GameEntityType, IGameEntity, Args<IGameContext>>(
                _entityCatalog,
                new Args<IGameContext>(context)
            ));
            context.AddEntityWorld(entityManager);
            context.BindManager(entityManager).EveryUpdate();
            
            this.InstallEntitySpace(context);
            this.InstallUnitSystems(context, entityManager);
            this.InstallProjectileSystems(context, entityManager);
        }

        private void InstallEntitySpace(IGameContext context)
        {
            SpatialHash2D<IGameEntity> spatialHash = new SpatialHash2D<IGameEntity>(cellSize: 32, 8000);
            context.AddEntitySpace(spatialHash);
            context.AddGizmosBehaviour(() => spatialHash.DrawGizmosXZ());
        }

        private void InstallUnitSystems(IGameContext context, EntityManager<IGameEntity> entityManager)
        {
            context.BindSystem(BudgetSystem<IGameEntity>
                .Using(new DetectTargetJob(context))
                .From(entityManager, e => e.HasDetectorTag())
                .Budget(4096)
                .Build()
            ).EveryFixedTick();

            context.BindFixedTickSystem(BudgetSystem<IGameEntity>
                .Using(new UpdateGameEntityPriorityJob(_unitPrioritySettings, Camera.main!.transform))
                .From(entityManager, e => e.HasUnitTag())
                .Budget(1024)
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, _) => entity.Attack())
                .From(context.GetEntityWorld(), entity => entity.HasAttackerTag())
                .Priority(GameEntityPriorityStrategy.Default)
                .Budget(1024)
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, deltaTime) => entity.ProcessMove(deltaTime))
                .From(entityManager, e => e.HasUnitTag() && e.HasMoveableTag())
                .Priority(GameEntityPriorityStrategy.Default)
                .Budget(1024)
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, deltaTime) => entity.ProcessFire(deltaTime))
                .From(entityManager, entity => entity.HasFireableTag())
                .Priority(GameEntityPriorityStrategy.Default)
                .Build()
            );
        }

        private void InstallProjectileSystems(IGameContext context, EntityManager<IGameEntity> entityManager)
        {
            var projectileFilter = new Filter<IGameEntity>(entityManager, e => e.HasProjectileTag());
           
            context.BindFixedTickSystem(BudgetSystem<IGameEntity>
                .Using(new ProjectileMoveJob(context))
                .From(projectileFilter)
                .Budget(1024)
                .Build()
                .AsAdaptive(_budgetStrategy)
            );

            context.BindFixedTickSystem(BudgetSystem<IGameEntity>
                .Using(new ProjectileLifetimeJob(context))
                .From(projectileFilter)
                .Budget(1024)
                .Build()
                .AsAdaptive(_budgetStrategy)
            );
        }
    }
}