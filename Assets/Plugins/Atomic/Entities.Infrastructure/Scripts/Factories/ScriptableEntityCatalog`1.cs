#if UNITY_5_3_OR_NEWER
using Atomic.Infrastructure;
using UnityEngine;

namespace Atomic.Entities
{
    /// <summary>
    /// A <see cref="ScriptableObject"/>-based catalog of entity factories.
    /// Provides creation and lookup of entities using a key.
    /// </summary>
    /// <typeparam name="TKey">Key used to identify factories (enum, string, etc).</typeparam>
    /// <typeparam name="E">Entity type (must implement <see cref="IEntity"/>).</typeparam>
    /// <typeparam name="TFactory">Factory type (inherits <see cref="ScriptableEntityFactory{TArgs}"/>).</typeparam>
    /// <typeparam name="TArgs">The type of construction arguments passed to each factory.</typeparam>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Factories/ScriptableEntityCatalog%601.md")]
    public abstract class ScriptableEntityCatalog<TKey, E, TFactory, TArgs> :
        ScriptableCatalog<TKey, E, TFactory, TArgs>
        where E : IEntity
        where TFactory : ScriptableObject, IFactory<E, TArgs>
        where TArgs : IArgs
    {
    }
}
#endif