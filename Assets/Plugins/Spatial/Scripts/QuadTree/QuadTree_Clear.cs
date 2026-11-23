namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public void Clear()
        {
            float minX = _root.minX;
            float minY = _root.minY;
            float maxX = _root.maxX;
            float maxY = _root.maxY;

            this.ClearRecursively(_root);

            _root = NodePool.Rent(minX, minY, maxX, maxY, _capacity, 0, null);

            _lookup.Clear();
        }
        
        private void ClearRecursively(Node node)
        {
            if (node == null)
                return;

            if (node.divided)
            {
                this.ClearRecursively(node.nw);
                this.ClearRecursively(node.ne);
                this.ClearRecursively(node.sw);
                this.ClearRecursively(node.se);
            }

            NodePool.Return(node);
        }
    }
}