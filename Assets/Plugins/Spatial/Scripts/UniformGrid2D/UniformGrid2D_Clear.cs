namespace Modules.Spatial
{
    public partial class UniformGrid2D<T>
    {
        public void Clear()
        {
            foreach (Cell t in _cells)
                t.Clear();

            _lookup.Clear();
        }
    }
}