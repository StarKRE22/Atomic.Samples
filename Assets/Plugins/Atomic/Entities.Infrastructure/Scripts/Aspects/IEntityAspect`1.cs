using System;
using Atomic.Infrastructure;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents an aspect of an entity that can be applied or discarded.
    /// </summary>
    /// <typeparam name="E">The type of entity (<see cref="IEntity"/>) this aspect can be applied to.</typeparam>
    [Obsolete]
    public interface IEntityAspect<in E> : IAspect<E> where E : IEntity
    {
    }
}