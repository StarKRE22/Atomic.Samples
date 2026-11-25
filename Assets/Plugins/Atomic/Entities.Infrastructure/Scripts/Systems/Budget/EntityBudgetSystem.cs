using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public abstract class EntityBudgetSystem : BudgetSystem<IEntity>
    {
        protected EntityBudgetSystem(IReadOnlyStore<IEntity> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }
    }

    public abstract class EntityBudgetSystem<E> : BudgetSystem<E> where E : IEntity
    {
        protected EntityBudgetSystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }
    }

    #region Tick

    public class EntityTickBudgetSystem : EntityTickBudgetSystem<IEntity>
    {
        public EntityTickBudgetSystem(IReadOnlyStore<IEntity> source, int budget = DEFAULT_BUDGET) :
            base(source, budget)
        {
        }
    }

    public class EntityTickBudgetSystem<E> : EntityBudgetSystem<E> where E : IEntity
    {
        public EntityTickBudgetSystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.Tick(deltaTime);
    }

    #endregion

    #region FixedTick

    public class EntityFixedTickBudgetSystem : EntityFixedTickBudgetSystem<IEntity>
    {
        public EntityFixedTickBudgetSystem(IReadOnlyStore<IEntity> source, int budget = DEFAULT_BUDGET) :
            base(source, budget)
        {
        }
    }

    public class EntityFixedTickBudgetSystem<E> : EntityBudgetSystem<E> where E : IEntity
    {
        public EntityFixedTickBudgetSystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source,
            budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.FixedTick(deltaTime);
    }

    #endregion

    #region LateTick

    public class EntityLateTickBudgetSystem : EntityLateTickBudgetSystem<IEntity>
    {
        public EntityLateTickBudgetSystem(IReadOnlyStore<IEntity> source, int budget = DEFAULT_BUDGET) :
            base(source, budget)
        {
        }
    }

    public class EntityLateTickBudgetSystem<E> : EntityBudgetSystem<E> where E : IEntity
    {
        public EntityLateTickBudgetSystem(IReadOnlyStore<E> source, int budget = DEFAULT_BUDGET) : base(source, budget)
        {
        }

        protected override void Update(E entity, float deltaTime) =>
            entity.LateTick(deltaTime);
    }

    #endregion
}