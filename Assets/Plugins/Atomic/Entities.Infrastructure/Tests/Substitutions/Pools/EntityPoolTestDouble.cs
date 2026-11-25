using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public sealed class EntityPoolTestDouble : IPool<IEntity>
    {
        public Action DisposeMethod;
        public Func<IEntity> RentMethod;
        public Action<IEntity> ReturnMethod;
        public Action<int> InitMethod;

        public void Dispose() => this.DisposeMethod.Invoke();

        public IEntity Rent() => this.RentMethod.Invoke();

        public void Return(IEntity entity) => this.ReturnMethod.Invoke(entity);

        public void Init(int initialCount) => this.InitMethod.Invoke(initialCount);
    }
}