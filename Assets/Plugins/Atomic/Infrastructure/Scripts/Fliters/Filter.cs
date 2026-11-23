using System;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Infrastructure
{
    public class Filter<T> : IReadOnlyStore<T>, IDisposable
    {
        public event Action OnStateChanged;
        public event Action<T> OnAdded;

        public event Action<T> OnRemoved;

        public int Count => this.state.Count;

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        private readonly Store<T> state;
        private readonly IReadOnlyStore<T> source;
        private readonly Predicate<T> predicate;
        private readonly ITracker<T>[] trackers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilter{E}"/> class.
        /// </summary>
        /// <param name="source">The source entity collection to observe.</param>
        /// <param name="predicate">The predicate used to determine filter inclusion.</param>
        /// <param name="trackers">Optional triggers used to re-evaluate entities when their state changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public Filter(
            IReadOnlyStore<T> source,
            Predicate<T> predicate,
            params ITracker<T>[] trackers
        )
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            this.state = new Store<T>();
            this.trackers = trackers;

            for (int i = 0, count = trackers.Length; i < count; i++)
                trackers[i].SetTrigger(this.Synchronize);

            this.source.OnAdded += this.Observe;
            this.source.OnRemoved += this.Unobserve;

            foreach (T entity in this.source)
                this.Observe(entity);
        }

        /// <summary>
        /// Releases all subscriptions and clears internal state.
        /// </summary>
        public void Dispose()
        {
            foreach (T entity in this.source)
                this.Unobserve(entity);

            this.source.OnAdded -= this.Observe;
            this.source.OnRemoved -= this.Unobserve;
        }

        /// <inheritdoc/>
        public void CopyTo(ICollection<T> results) => this.state.CopyTo(results);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => this.state.CopyTo(array, arrayIndex);

        public T this[int index] => state[index];

        public bool TryGetAt(int index, out T entity) => state.TryGetAt(index, out entity);

        /// <inheritdoc/>
        public bool Contains(T entity) => this.state.Contains(entity);

        public Store<T>.Enumerator GetEnumerator() => this.state.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.state.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void Observe(T entity)
        {
            for (int i = 0, count = this.trackers.Length; i < count; i++)
                this.trackers[i].Track(entity);

            if (this.predicate(entity) && this.state.Add(entity))
            {
                this.OnStateChanged?.Invoke();
                this.OnAdded?.Invoke(entity);
            }
        }

        private void Unobserve(T entity)
        {
            for (int i = 0, count = this.trackers.Length; i < count; i++)
                this.trackers[i].Untrack(entity);

            if (this.state.Remove(entity))
            {
                this.OnStateChanged?.Invoke();
                this.OnRemoved?.Invoke(entity);
            }
        }

        internal void Synchronize(T entity)
        {
            bool matches = this.predicate(entity);

            if (!matches && this.state.Remove(entity))
            {
                this.OnStateChanged?.Invoke();
                this.OnRemoved?.Invoke(entity);
            }
            else if (matches && this.state.Add(entity))
            {
                this.OnStateChanged?.Invoke();
                this.OnAdded?.Invoke(entity);
            }
        }

        public static Builder StartBuild() => new();

        /// <summary>
        /// Fluent builder for creating <see cref="EntityFilter{E}"/> instances.
        /// </summary>
        public struct Builder
        {
            private IReadOnlyStore<T> _source;
            private Predicate<T> _predicate;
            private List<ITracker<T>> _triggers;

            /// <summary>
            /// Assigns a source collection.
            /// </summary>
            public Builder WithSource(IReadOnlyStore<T> source)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
                return this;
            }

            /// <summary>
            /// Assigns a filter predicate.
            /// </summary>
            public Builder WithPredicate(Predicate<T> predicate)
            {
                _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
                return this;
            }

            /// <summary>
            /// Adds a trigger.
            /// </summary>
            public Builder WithTrigger(ITracker<T> tracker)
            {
                if (tracker == null) throw new ArgumentNullException(nameof(tracker));

                _triggers ??= new List<ITracker<T>>();
                _triggers.Add(tracker);
                return this;
            }

            /// <summary>
            /// Adds multiple triggers.
            /// </summary>
            public Builder WithTriggers(params ITracker<T>[] triggers)
            {
                if (triggers == null) throw new ArgumentNullException(nameof(triggers));

                _triggers ??= new List<ITracker<T>>();
                _triggers.AddRange(triggers);
                return this;
            }

            /// <summary>
            /// Builds and returns the configured <see cref="EntityFilter{E}"/>.
            /// </summary>
            public Filter<T> Build()
            {
                if (_source == null) throw new InvalidOperationException("Source must be provided.");
                if (_predicate == null) throw new InvalidOperationException("Predicate must be provided.");

                return new Filter<T>(
                    _source,
                    _predicate,
                    _triggers != null ? _triggers.ToArray() : Array.Empty<ITracker<T>>()
                );
            }
        }
    }
}