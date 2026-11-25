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
            EntityWorld<IGameEntity> entityWorld = new EntityWorld<IGameEntity>();
            context.AddEntityPool(new MultiPool<GameEntityType, IGameEntity, Args<IGameContext>>(
                _entityCatalog,
                new Args<IGameContext>(context)
            ));
            context.AddEntityWorld(entityWorld);
            context.BindWorld(entityWorld).EveryUpdate();
            
            this.InstallEntitySpace(context);
            this.InstallUnitSystems(context, entityWorld);
            this.InstallProjectileSystems(context, entityWorld);
        }

        private void InstallEntitySpace(IGameContext context)
        {
            SpatialHash2D<IGameEntity> spatialHash = new SpatialHash2D<IGameEntity>(cellSize: 32, 8000);
            context.AddEntitySpace(spatialHash);
            context.AddGizmosBehaviour(() => spatialHash.DrawGizmosXZ());
        }

        private void InstallUnitSystems(IGameContext context, EntityWorld<IGameEntity> entityWorld)
        {
            context.BindSystem(BudgetSystem<IGameEntity>
                .Using(new DetectTargetJob(context))
                .From(entityWorld, e => e.HasDetectorTag())
                .Budget(4096)
                .Build()
            ).EveryFixedTick();

            context.BindFixedTickSystem(BudgetSystem<IGameEntity>
                .Using(new UpdateGameEntityPriorityJob(_unitPrioritySettings, Camera.main!.transform))
                .From(entityWorld, e => e.HasUnitTag())
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
                .From(entityWorld, e => e.HasUnitTag() && e.HasMoveableTag())
                .Priority(GameEntityPriorityStrategy.Default)
                .Budget(1024)
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, deltaTime) => entity.ProcessFire(deltaTime))
                .From(entityWorld, entity => entity.HasFireableTag())
                .Priority(GameEntityPriorityStrategy.Default)
                .Build()
            );
        }

        private void InstallProjectileSystems(IGameContext context, EntityWorld<IGameEntity> entityWorld)
        {
            var projectileFilter = new Filter<IGameEntity>(entityWorld, e => e.HasProjectileTag());
           
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