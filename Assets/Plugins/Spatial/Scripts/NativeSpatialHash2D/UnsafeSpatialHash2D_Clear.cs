using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Modules.Spatial
{
    public partial struct UnsafeSpatialHash2D<T>
    {
        public void Clear()
        {
            foreach (KVPair<long, UnsafeList<Entry>> pair in _cells) 
                pair.Value.Dispose();

            _cells.Clear();
            _lookup.Clear();
        }
    }
}