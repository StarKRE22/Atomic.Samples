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

        [SerializeField]
        private int sizeX = 2000;

        [SerializeField]
        private int sizeY = 100;

        [SerializeField]
        private float cellSize = 20;

        public void Install(IGameContext context)
        {
            EntityWorld<IGameEntity> entityWorld = new EntityWorld<IGameEntity>();
            context.AddEntityPool(new MultiEntityPool<GameEntityType, IGameEntity, Args<IGameContext>>(
                _entityCatalog,
                new Args<IGameContext>(context)
            ));
            context.AddEntityWorld(entityWorld);
            UniformGrid2D<IGameEntity> grid = new UniformGrid2D<IGameEntity>(sizeX, sizeY, cellSize);
            context.AddGizmosBehaviour(() => grid.DrawGizmosXZ());
            context.AddEntitySpace(grid);
            
            context.BindWorld(entityWorld);
            context.BindFixedTickSystem(new UnitPrioritySystem(context, _unitPrioritySettings));
            context.BindFixedTickSystem(new AttackTargetSystem(context));
            context.BindFixedTickSystem(new DetectTargetSystem(context));
            context.BindFixedTickSystem(new MoveUnitSystem(context));
            context.BindFixedTickSystem(new FireUnitsSystem(context));
            context.BindFixedTickSystem(new ProjectileMoveSystem(context).AsAdaptive(_budgetStrategy));
            context.BindFixedTickSystem(new ProjectileLifetimeSystem(context).AsAdaptive(_budgetStrategy));
        }
    }
}