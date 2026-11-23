using System.Collections.Generic;
using UnityEngine;

namespace Atomic.Infrastructure
{
    public abstract class ViewCatalog<TModel, TView> : ScriptableObject
        where TModel : class
        where TView : View<TModel>
    {
        /// <summary>
        /// The list of entity view prefabs available in this catalog.
        /// </summary>
        [Tooltip("The list of view prefabs available in this catalog")]
        [SerializeField]
        private List<TView> prefabs;

        /// <summary>
        /// Gets the number of prefabs stored in the catalog.
        /// </summary>
        public int Count => this.prefabs.Count;

        public TView GetPrefab(int index) => prefabs[index];
    }
}