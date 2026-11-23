using System.Text;
// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        private StringBuilder _stringBuilder;

        public override string ToString()
        {
            _stringBuilder ??= new StringBuilder(256);
            _stringBuilder.Clear();
            this.ToStringInternal(root, _stringBuilder, 0, "Root");
            return _stringBuilder.ToString();
        }

        private void ToStringInternal(Node node, StringBuilder sb, int depth, string prefix)
        {
            if (node == null)
                return;

            sb.AppendLine(new string(' ', depth * 4) +
                          $"{prefix}:[{node.point}] → Count:{node.values.Count}");

            this.ToStringInternal(node.left, sb, depth + 1, "L");
            this.ToStringInternal(node.right, sb, depth + 1, "R");
        }
    }
}