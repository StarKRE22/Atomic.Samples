using System.Collections.Generic;
using System.Text;
using UnityEngine;
// ReSharper disable TailRecursiveCall

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        private StringBuilder _stringBuilder;

        public override string ToString()
        {
            _stringBuilder ??= new StringBuilder(256);
            _stringBuilder.Clear();
            this.ToStringInternal(_root, _stringBuilder, "", "ROOT");
            return _stringBuilder.ToString();
        }

        private void ToStringInternal(Node node, StringBuilder sb, string indent, string name)
        {
            if (node == null)
                return;

            sb.Append(indent);
            sb.Append(name);
            sb.Append(" [D:");
            sb.Append(node.depth);
            sb.Append("] ");

            sb.Append("Bounds=(");
            sb.Append(node.minX);
            sb.Append(",");
            sb.Append(node.minY);
            sb.Append(")-(");
            sb.Append(node.maxX);
            sb.Append(",");
            sb.Append(node.maxY);
            sb.Append(")");

            sb.Append(" Items=");
            sb.Append(node.items.Count);

            sb.AppendLine();

            foreach (KeyValuePair<T, Vector2> entry in node.items)
            {
                sb.Append(indent);
                sb.Append("  • ");

                sb.Append(entry.Key);

                sb.Append(" @ ");

                sb.Append(entry.Value);

                sb.AppendLine();
            }

            if (!node.divided)
                return;

            this.ToStringInternal(node.nw, sb, indent + "  ", "NW");
            this.ToStringInternal(node.ne, sb, indent + "  ", "NE");
            this.ToStringInternal(node.sw, sb, indent + "  ", "SW");
            this.ToStringInternal(node.se, sb, indent + "  ", "SE");
        }
    }
}