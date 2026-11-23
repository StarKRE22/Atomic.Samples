using System.Collections.Generic;
using System.Linq;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class EntityCollection<E> : ObjectCollection<E>, IEntityCollection<E> where E : IEntity
    {
        public EntityCollection() : this(DEFAULT_CAPACITY)
        {
        }

        public EntityCollection(int capacity) : base(capacity)
        {
        }

        public EntityCollection(IEnumerable<E> elements) : this(elements.Count()) => this.AddRange(elements);

        public EntityCollection(params E[] entities) : this(entities.Length) => this.AddRange(entities);

        public EntityCollection(IReadOnlyCollection<E> elements) : this(elements.Count) => this.AddRange(elements);
    }
}