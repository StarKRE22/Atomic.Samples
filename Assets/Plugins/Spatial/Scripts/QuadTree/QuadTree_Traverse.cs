// ReSharper disable TailRecursiveCall
namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public delegate void TraverseHandler(float minX, float minY, float maxX, float maxY, int count);
        
        public void Traverse(TraverseHandler visitor)
        {
            Traverse(_root, visitor);
        }

        private static void Traverse(Node node, TraverseHandler visitor)
        {
            if (node == null)
                return;

            visitor(node.minX, node.minY, node.maxX, node.maxY, node.items.Count);

            Traverse(node.nw, visitor);
            Traverse(node.ne, visitor);
            Traverse(node.sw, visitor);
            Traverse(node.se, visitor);
        }
    }
}