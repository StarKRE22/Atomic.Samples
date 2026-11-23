using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialList2D<T>
    {
        public Enumerator GetEnumerator() =>
            new(_entries, _count);

        IEnumerator<KeyValuePair<T, Vector2>> IEnumerable<KeyValuePair<T, Vector2>>.GetEnumerator() =>
            new Enumerator(_entries, _count);

        IEnumerator IEnumerable.GetEnumerator() =>
            new Enumerator(_entries, _count);

        public struct Enumerator : IEnumerator<KeyValuePair<T, Vector2>>
        {
            private readonly KeyValuePair<T, Vector2>[] _entries;
            private readonly int _count;

            private int _index;

            public KeyValuePair<T, Vector2> Current { get; private set; }

            object IEnumerator.Current => Current;

            internal Enumerator(KeyValuePair<T, Vector2>[] entries, int count)
            {
                _entries = entries;
                _count = count;
                _index = -1;
                
                this.Current = default;
            }

            public bool MoveNext()
            {
                int next = _index + 1;
                if (next >= _count)
                    return false;

                _index = next;
                
                this.Current = _entries[next];
                return true;
            }

            public void Reset()
            {
                _index = -1;
                this.Current = default;
            }

            public void Dispose()
            {
            }
        }
    }
}