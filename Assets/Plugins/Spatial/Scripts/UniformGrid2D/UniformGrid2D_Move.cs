using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public bool Move(T obj, Vector2 p)
        {
            if (!_lookup.TryGetValue(obj, out Handle handle))
                return false;

            if (!this.TryGetIndex(p, out int newCellIndex))
                return false;

            int oldCellIndex = handle.cellIndex;
            if (oldCellIndex == newCellIndex)
            {
                Cell cell = _cells[oldCellIndex];
                cell.SetPosition(handle.indexInCell, p);
                return true;
            }

            this.RemoveInternal(obj, handle);

            Cell newCell = _cells[newCellIndex];
            int newIndex = newCell.Add(new KeyValuePair<T, Vector2>(obj, p));

            _lookup[obj] = new Handle
            {
                cellIndex = newCellIndex,
                indexInCell = newIndex
            };

            return true;
        }
    }
}