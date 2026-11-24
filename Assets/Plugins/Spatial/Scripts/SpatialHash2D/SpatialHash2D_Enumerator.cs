using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class SpatialHash2D<T>
    {
        public Enumerator GetEnumerator() => new(_cells);

        IEnumerator<KeyValuePair<T, Vector2>> IEnumerable<KeyValuePair<T, Vector2>>.GetEnumerator() => 
            new Enumerator(_cells);

        IEnumerator IEnumerable.GetEnumerator() =>
            new Enumerator(_cells);

        public struct Enumerator : IEnumerator<KeyValuePair<T, Vector2>>
        {
            private Dictionary<Vector2Int, List<KeyValuePair<T, Vector2>>>.Enumerator _cellEnumerator;
            private List<KeyValuePair<T, Vector2>> _currentList;
            private int _index;

            public KeyValuePair<T, Vector2> Current { get; private set; }

            object IEnumerator.Current => Current;

            internal Enumerator(Dictionary<Vector2Int, List<KeyValuePair<T, Vector2>>> cells)
            {
                _cellEnumerator = cells.GetEnumerator();

                _currentList = null;
                _index = -1;

                Current = default;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_currentList != null)
                    {
                        _index++;

                        if (_index < _currentList.Count)
                        {
                            Current = _currentList[_index];
                            return true;
                        }
                    }

                    if (!_cellEnumerator.MoveNext())
                        return false;

                    _currentList = _cellEnumerator.Current.Value;
                    _index = -1;
                }
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
            }
        }
    }
}