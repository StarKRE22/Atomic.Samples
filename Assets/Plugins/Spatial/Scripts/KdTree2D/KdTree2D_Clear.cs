namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public void Clear()
        {
            this.ClearRecursive(root);
            root = null;
            _count = 0;
            
            _lookup.Clear();
        }

        private void ClearRecursive(Node node)
        {
            if (node == null)
                return;

            this.ClearRecursive(node.left);
            this.ClearRecursive(node.right);

            NodePool.Return(node);
        }
    }
}