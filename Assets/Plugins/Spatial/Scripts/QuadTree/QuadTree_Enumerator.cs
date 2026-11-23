using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Spatial
{
    public partial class QuadTree<T>
    {
        public Enumerator GetEnumerator() => 
            new(_root);

        IEnumerator<KeyValuePair<T, Vector2>> IEnumerable<KeyValuePair<T, Vector2>>.GetEnumerator() => 
            this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public struct Enumerator : IEnumerator<KeyValuePair<T, Vector2>>
        {
            private Node[] _stack;
            private int _stackCount;

            private Node _currentNode;
            private int _itemIndex;

            public KeyValuePair<T, Vector2> Current { get; private set; }

            object IEnumerator.Current => Current;

            internal Enumerator(Node root)
            {
                _stack = ArrayPool<Node>.Shared.Rent(32);
                _stackCount = 0;

                if (root != null)
                    _stack[_stackCount++] = root;

                _currentNode = null;
                _itemIndex = -1;
                Current = default;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_currentNode != null)
                    {
                        int nextIndex = _itemIndex + 1;
                        if (nextIndex < _currentNode.items.Count)
                        {
                            _itemIndex = nextIndex;
                            Current = _currentNode.items[nextIndex];
                            return true;
                        }

                        _currentNode = null;
                    }

                    if (_stackCount == 0)
                        return false;

                    Node node = _stack[--_stackCount];
                    if (node.divided)
                    {
                        this.Push(node.nw);
                        this.Push(node.ne);
                        this.Push(node.sw);
                        this.Push(node.se);
                    }

                    if (node.items.Count > 0)
                    {
                        _currentNode = node;
                        _itemIndex = -1;
                    }
                }
            }

            private void Push(Node node)
            {
                if (node == null)
                    return;

                if (_stackCount == _stack.Length)
                {
                    Node[] newStack = ArrayPool<Node>.Shared.Rent(_stack.Length * 2);
                    Array.Copy(_stack, newStack, _stack.Length);
                    ArrayPool<Node>.Shared.Return(_stack, true);
                    _stack = newStack;
                }

                _stack[_stackCount++] = node;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
                if (_stack == null)
                    return;

                ArrayPool<Node>.Shared.Return(_stack, true);
                _stack = null;
                _stackCount = 0;
                _currentNode = null;
            }
        }
    }
}