using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public Enumerator GetEnumerator() =>
            new(_cells);

        IEnumerator<KeyValuePair<T, Vector2>> IEnumerable<KeyValuePair<T, Vector2>>.GetEnumerator() =>
            new Enumerator(_cells);

        IEnumerator IEnumerable.GetEnumerator() =>
            new Enumerator(_cells);

        public struct Enumerator : IEnumerator<KeyValuePair<T, Vector2>>
        {
            private readonly Cell[] _cells;

            private int _cellIndex;
            private int _itemIndex;

            public KeyValuePair<T, Vector2> Current { get; private set; }

            object IEnumerator.Current => Current;

            internal Enumerator(Cell[] cells)
            {
                _cells = cells;
                _cellIndex = 0;
                _itemIndex = -1;

                Current = default;
            }

            public bool MoveNext()
            {
                while (_cellIndex < _cells.Length)
                {
                    Cell cell = _cells[_cellIndex];
                    List<KeyValuePair<T, Vector2>> items = cell.items;

                    _itemIndex++;
                    if (_itemIndex < items.Count)
                    {
                        Current = items[_itemIndex];
                        return true;
                    }

                    _cellIndex++;
                    _itemIndex = -1;
                }

                return false;
            }

            public void Reset()
            {
                _cellIndex = 0;
                _itemIndex = -1;
                Current = default;
            }

            public void Dispose()
            {
            }
        }
    }
}