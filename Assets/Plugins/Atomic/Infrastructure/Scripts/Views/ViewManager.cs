using System;

namespace Atomic.Infrastructure
{
    public abstract class ViewManager<TKey, TModel, TView> : ViewStore<TKey, TModel, TView>
        where TModel : class
        where TView : View<TModel>
    {
        /// <summary>
        /// Gets a value indicating whether this collection is currently visible 
        /// (i.e. has a bound <see cref="IReadOnlyEntityCollection{E}"/> source).
        /// </summary>
        public bool IsActive => _source != null;

        private IReadOnlyStore<TModel> _source;

        /// <summary>
        /// Shows this collection, binding it to the specified source of entities.
        /// </summary>
        /// <param name="source">The entity collection to visualize.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public void Show(IReadOnlyStore<TModel> source)
        {
            this.Hide();

            _source = source ?? throw new ArgumentNullException(nameof(source));
            
            foreach (TModel entity in _source) 
                this.Add(entity);

            _source.OnAdded += this.OnAdd;
            _source.OnRemoved += this.OnRemove;
        }

        /// <summary>
        /// Hides this collection, detaching it from the bound entity source and removing all views.
        /// </summary>
        public void Hide()
        {
            if (_source != null)
            {
                _source.OnAdded -= this.OnAdd;
                _source.OnRemoved -= this.OnRemove;
            }

            this.Clear();
            _source = null;
        }

        private void OnAdd(TModel entity) => this.Add(entity);
        
        private void OnRemove(TModel entity) => this.Remove(entity);
    }
}