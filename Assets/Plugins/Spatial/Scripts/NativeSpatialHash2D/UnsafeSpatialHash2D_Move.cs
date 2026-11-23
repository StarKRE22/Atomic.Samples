using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Modules.Spatial
{
    public partial struct UnsafeSpatialHash2D<T>
    {
        public bool Move(T item, float2 position)
        {
            if (!_lookup.TryGetValue(item, out Handle handle))
                return false;

            long newCellKey = GetKey(position);
            if (handle.cellKey == newCellKey)
            {
                UnsafeList<Entry> cell = _cells[handle.cellKey];
                cell[handle.index] = new Entry(item, position);
                _cells[handle.cellKey] = cell;

                return true;
            }

            this.RemoveInternal(handle);

            if (!_cells.TryGetValue(newCellKey, out UnsafeList<Entry> newCell))
            {
                newCell = new UnsafeList<Entry>(_initialCellCapacity, _allocator);
                _cells.Add(newCellKey, newCell);
            }

            int newIndex = newCell.Length;
            newCell.Add(new Entry(item, position));

            _cells[newCellKey] = newCell;
            _lookup[item] = new Handle(newCellKey, newIndex);
            return true;
        }
    }
}