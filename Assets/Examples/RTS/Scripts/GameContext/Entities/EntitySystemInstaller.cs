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
        private UpdatePriorityProcessor.Settings _unitPrioritySettings;

        [SerializeField]
        private ScriptableBudgetStrategy _budgetStrategy;

        public void Install(IGameContext context)
        {
            EntityWorld<IGameEntity> entityWorld = new EntityWorld<IGameEntity>();
            context.AddEntityPool(new MultiEntityPool<GameEntityType, IGameEntity, Args<IGameContext>>(
                _entityCatalog,
                new Args<IGameContext>(context)
            ));
            context.AddEntityWorld(entityWorld);

            QuadTree<IGameEntity> quadTree = new QuadTree<IGameEntity>(
                min: new Vector2(-250, -250),
                max: new Vector2(500, 500),
                capacity: 32,
                maxDepth: 8
            );

            context.AddEntitySpace(quadTree);
            context.AddGizmosBehaviour(() => quadTree.DrawGizmosXZ());

            // context.AddGizmosBehaviour(() => kdTree.DrawGizmosXZ(new Rect(Vector2.zero, new Vector2(4000, 200))));

            context.BindWorld(entityWorld);
            context.BindSystem(BudgetSystem<IGameEntity>
                .Using(new DetectTargetProcessor(context))
                .From(entityWorld, e => e.HasDetectorTag())
                .Budget(4096)
                .Build()
            ).EveryFixedTick();

            context.BindFixedTickSystem(BudgetSystem<IGameEntity>
                .Using(new UpdatePriorityProcessor(_unitPrioritySettings, Camera.main!.transform))
                .From(entityWorld, e => e.HasUnitTag())
                .Budget(1024)
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, _) => entity.Attack())
                .From(context.GetEntityWorld(), entity => entity.HasAttackerTag())
                .Budget(1024)
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, deltaTime) => entity.ProcessMove(deltaTime))
                .From(entityWorld, e => e.HasUnitTag() && e.HasMoveableTag())
                .Build()
            );

            context.BindFixedTickSystem(PriorityBudgetSystem<IGameEntity>
                .Using((entity, deltaTime) => entity.ProcessFire(deltaTime))
                .From(entityWorld, entity => entity.HasFireableTag())
                .Build());
            
            context.BindFixedTickSystem();
            
            context.BindFixedTickSystem(new ProjectileMoveProcessor(context).AsAdaptive(_budgetStrategy));
            context.BindFixedTickSystem(new ProjectileLifetimeSystem(context).AsAdaptive(_budgetStrategy));
        }
        
        // new EntityFilter<IGameEntity>(context.GetEntityWorld(), e => e.HasProjectileTag())
//         
//         using System;
//         using Atomic.Entities;
//         using Atomic.Infrastructure;
//         using UnityEngine;
//
//         namespace RTSGame
//         {
//             [Serializable]
//             public sealed class ProjectileMoveSystem : GameEntityBudgetSystem
//         {
//         private readonly IGameContext _gameContext;
//
//         public ProjectileMoveSystem(IGameContext context)
//             : base(new EntityFilter<IGameEntity>(context.GetEntityWorld(), e => e.HasProjectileTag()))
//         {
//             _gameContext = context;
//         }
//
//         protected override void Update(IGameEntity entity, float deltaTime)
//         {
//             IGameEntity target = entity.GetTarget().Value;
//             if (target is not {Enabled: true})
//             {
//                 entity.MoveStep(entity.GetForwardDirection(), deltaTime);
//                 return;
//             }
//
//             Vector3 vector = entity.GetDistanceVector(target);
//             float scale = entity.GetScale().Value;
//             if (vector.sqrMagnitude > scale * scale)
//             {
//                 entity.GetRotation().Value = Quaternion.LookRotation(vector.normalized);
//                 entity.MoveStep(vector.normalized, deltaTime);
//             }
//             else if (entity.DealDamage(target))
//                 _gameContext.Despawn(entity);
//         }
//     }
// }
    }
}