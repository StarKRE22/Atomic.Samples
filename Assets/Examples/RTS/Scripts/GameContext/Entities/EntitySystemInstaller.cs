using System;
using Atomic.Entities;
using Atomic.Infrastructure;
using Modules.SpatialStructures;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public sealed class EntitySystemInstaller : IEntityInstaller<IGameContext>
    {
        [SerializeField]
        private GameEntityCatalog _entityCatalog;

        [SerializeField]
        private UnitPrioritySettings _unitPrioritySettings;

        [SerializeField]
        private GamePriorityEntitySystemSettings _attackTargetSettings;

        [SerializeField]
        private GamePriorityEntitySystemSettings _detectTargetSettings;

        [SerializeField]
        private GamePriorityEntitySystemSettings _moveUnitSettings;

        [SerializeField]
        private GamePriorityEntitySystemSettings _fireUnitsSettings;

        [SerializeField]
        private GamePriorityEntitySystemSettings _projectileMoveSettings;

        [SerializeField]
        private GamePriorityEntitySystemSettings _projectileLifetimeSettings;

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
                new Args<IGameContext>(context))
            );
            context.AddEntityWorld(entityWorld);
            context.AddEntitySpace(new SpatialGrid2D<IGameEntity>(sizeX, sizeY, cellSize));

            context.AddFixedTickSystem(new UnitPrioritySystem(context, _unitPrioritySettings));
            context.AddFixedTickSystem(new AttackTargetSystem(context, _attackTargetSettings));
            context.AddFixedTickSystem(new DetectTargetSystem(context, _detectTargetSettings));
            context.AddFixedTickSystem(new MoveUnitSystem(context, _moveUnitSettings));
            context.AddFixedTickSystem(new FireUnitsSystem(context, _fireUnitsSettings));
            context.AddFixedTickSystem(new ProjectileMoveSystem(context, _projectileMoveSettings));
            context.AddFixedTickSystem(new ProjectileLifetimeSystem(context, _projectileLifetimeSettings));

            entityWorld.BindTo(context);
        }
    }
}
