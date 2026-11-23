using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Modules.Spatial
{
    public partial struct UnsafeSpatialHash2D<T>
    {
        public bool Add(in T item, float2 position)
        {
            if (_lookup.ContainsKey(item))
                return false;

            long key = this.GetKey(position);
            if (_cells.TryGetValue(key, out UnsafeList<Entry> cell))
            {
                int index = cell.Length;
                cell.Add(new Entry(item, position));
                _cells[key] = cell; 
                _lookup.Add(item, new Handle(key, index));
            }
            else
            {
                cell = new UnsafeList<Entry>(_initialCellCapacity, _allocator);
                cell.Add(new Entry(item, position));
                _cells.Add(key, cell);
                _lookup.Add(item, new Handle(key, 0));
            }

            return true;
        }
    }
}