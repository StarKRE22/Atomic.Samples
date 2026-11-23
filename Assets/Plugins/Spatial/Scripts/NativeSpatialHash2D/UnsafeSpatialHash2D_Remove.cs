using Unity.Collections.LowLevel.Unsafe;

namespace Modules.Spatial
{
    public partial struct UnsafeSpatialHash2D<T>
    {
        public bool Remove(in T item)
        {
            if (!_lookup.TryGetValue(item, out Handle handle))
                return false;

            this.RemoveInternal(handle);
            _lookup.Remove(item);

            return true;
        }

        private void RemoveInternal(Handle handle)
        {
            UnsafeList<Entry> cell = _cells[handle.cellKey];

            int lastIndex = cell.Length - 1;
            if (handle.index != lastIndex)
            {
                Entry movedEntry = cell[lastIndex];

                cell[handle.index] = movedEntry;
                Handle movedHandle = _lookup[movedEntry.Item];
                _lookup[movedEntry.Item] = new Handle(movedHandle.cellKey, handle.index);
            }

            cell.RemoveAt(lastIndex);

            if (cell.Length == 0)
            {
                cell.Dispose();
                _cells.Remove(handle.cellKey);
                return;
            }

            _cells[handle.cellKey] = cell;
        }
    }
}