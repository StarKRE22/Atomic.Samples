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
        private UnitPrioritySystem.Settings _unitPrioritySettings;

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
            context.BindFixedTickSystem(new BudgetSystem<IGameEntity>(
                new EntityFilter<IGameEntity>(entityWorld, e => e.HasDetectorTag()),
                new DetectTargetUpdater(context),
                budget: 4096
            ));
            context.BindFixedTickSystem(new UnitPrioritySystem(context, _unitPrioritySettings));
            context.BindFixedTickSystem(new AttackTargetSystem(context));
            context.BindFixedTickSystem(new MoveUnitSystem(context));
            context.BindFixedTickSystem(new FireUnitsSystem(context));
            context.BindFixedTickSystem(new ProjectileMoveSystem(context).AsAdaptive(_budgetStrategy));
            context.BindFixedTickSystem(new ProjectileLifetimeSystem(context).AsAdaptive(_budgetStrategy));

            //new DetectTargetUpdater(context)

            // private readonly IEntityWorld<IGameEntity> _entityWorld;
// private readonly IGameContext _gameContext;
//
// public DetectTargetUpdater(IGameContext context, int budget = 4096) :
// base(new EntityFilter<IGameEntity>(context.GetEntityWorld(),
//     e => e.HasDetectorTag()), budget)
// {
//     _gameContext = context;
//     _entityWorld = context.GetEntityWorld();
// }
        }
    }
}