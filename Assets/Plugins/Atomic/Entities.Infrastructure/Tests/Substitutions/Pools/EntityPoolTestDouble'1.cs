using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public sealed class EntityPoolTestDouble<T> : IPool<T> where T : IEntity
    {
        public Action DisposeMethod;
        public Func<T> RentMethod;
        public Action<T> ReturnMethod;
        public Action<int> InitMethod;
        
        public void Dispose() => this.DisposeMethod.Invoke();

        public T Rent() => this.RentMethod.Invoke();

        public void Return(T entity) => this.ReturnMethod.Invoke(entity);

        public void Init(int initialCount) => this.InitMethod.Invoke(initialCount);
    }
}