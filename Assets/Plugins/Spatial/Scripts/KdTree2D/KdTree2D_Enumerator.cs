using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules.Spatial
{
    public partial class KdTree2D<T>
    {
        public Enumerator GetEnumerator() =>
            new(this.root);

        IEnumerator<KeyValuePair<T, Vector2>> IEnumerable<KeyValuePair<T, Vector2>>.GetEnumerator() =>
            this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        public struct Enumerator : IEnumerator<KeyValuePair<T, Vector2>>
        {
            private readonly List<Node> _stack;

            private Node _currentNode;
            private int _valueIndex;

            public KeyValuePair<T, Vector2> Current { get; private set; }

            object IEnumerator.Current => Current;

            internal Enumerator(Node root)
            {
                _stack = ListPool<Node>.Get();

                _currentNode = null;
                _valueIndex = -1;

                Current = default;

                if (root != null)
                    _stack.Add(root);
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_currentNode != null)
                    {
                        _valueIndex++;

                        if (_valueIndex < _currentNode.values.Count)
                        {
                            Current = new KeyValuePair<T, Vector2>(
                                _currentNode.values[_valueIndex],
                                _currentNode.point
                            );

                            return true;
                        }

                        _currentNode = null;
                    }

                    int count = _stack.Count;
                    if (count == 0)
                        return false;

                    Node node = _stack[count - 1];
                    _stack.RemoveAt(count - 1);

                    if (node.right != null)
                        _stack.Add(node.right);

                    if (node.left != null)
                        _stack.Add(node.left);

                    _currentNode = node;
                    _valueIndex = -1;
                }
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                ListPool<Node>.Release(_stack);
            }
        }
    }
}