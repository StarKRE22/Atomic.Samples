using System.Collections.Generic;

namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public bool Remove(T obj)
        {
            if (!_lookup.TryGetValue(obj, out Handle item))
                return false;

            this.RemoveInternal(obj, item);
            _lookup.Remove(obj);
            return true;
        }

        private void RemoveInternal(T obj, Handle item)
        {
            ref Cell cell = ref _cells[item.cellIndex];
            cell.RemoveAtSwapBack(item.indexInCell, out var moved);

            T key = moved.Key;
            if (!EqualityComparer<T>.Default.Equals(key, obj))
            {
                _lookup[key] = new Handle
                {
                    cellIndex = item.cellIndex,
                    indexInCell = item.indexInCell
                };
            }
        }
    }
}