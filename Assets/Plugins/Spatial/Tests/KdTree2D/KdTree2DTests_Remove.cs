using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed partial class KdTree2DTests
    {
        [Test]
        public void RemoveRoot1()
        {
            object a = new();
            object b = new();
            object c = new();

            tree2D.Add(a, new Vector2(4, 6));
            tree2D.Add(b, new Vector2(-1, -2));
            tree2D.Add(c, new Vector2(5, 3));

            bool removed = tree2D.Remove(a);

            Assert.IsTrue(removed);
            Assert.AreEqual(2, tree2D.Count);
        }

        [Test]
        public void RemoveRoot2()
        {
            object a = new();
            object b = new();
            object c = new();
            object d = new();
            object e = new();
            object f = new();

            tree2D.Add(a, new Vector2(4, 6));
            tree2D.Add(b, new Vector2(-1, -2));
            tree2D.Add(c, new Vector2(5, 3));
            tree2D.Add(d, new Vector2(-2, 3));
            tree2D.Add(e, new Vector2(7, 2));
            tree2D.Add(f, new Vector2(7, 3));

            bool removed = tree2D.Remove(a);

            Assert.IsTrue(removed);
            Assert.AreEqual(5, tree2D.Count);
        }

        [Test]
        public void Remove_NodeExists_DecreasesCount()
        {
            object value = new();

            tree2D.Add(value, new Vector2(5, 5));

            bool removed = tree2D.Remove(value);

            Assert.IsTrue(removed);
            Assert.AreEqual(0, tree2D.Count);
        }

        [Test]
        public void Remove_NodeDoesNotExist_ReturnsFalse()
        {
            tree2D.Add(new object(), new Vector2(5, 5));

            bool removed = tree2D.Remove(new object());

            Assert.IsFalse(removed);
            Assert.AreEqual(1, tree2D.Count);
        }

        [Test]
        public void Remove_LeafNode_DeletesItSuccessfully()
        {
            object a = new();
            object b = new();

            tree2D.Add(a, new Vector2(5, 5));
            tree2D.Add(b, new Vector2(3, 3));

            bool removed = tree2D.Remove(b);

            Assert.IsTrue(removed);
            Assert.AreEqual(1, tree2D.Count);
        }

        [Test]
        public void Remove_NodeWithOneChild_ReplacesWithChild()
        {
            object a = new();
            object b = new();
            object c = new();

            tree2D.Add(a, new Vector2(5, 5));
            tree2D.Add(b, new Vector2(3, 3));
            tree2D.Add(c, new Vector2(2, 2));

            bool removed = tree2D.Remove(b);

            Assert.IsTrue(removed);
            Assert.AreEqual(2, tree2D.Count);
        }

        [Test]
        public void Remove_NodeWithTwoChildren_ReplacesWithMinimumFromRightSubtree()
        {
            object a = new();
            object b = new();
            object c = new();
            object d = new();
            object e = new();

            tree2D.Add(a, new Vector2(5, 5));
            tree2D.Add(b, new Vector2(3, 3));
            tree2D.Add(c, new Vector2(7, 7));
            tree2D.Add(d, new Vector2(6, 6));
            tree2D.Add(e, new Vector2(8, 8));

            bool removed = tree2D.Remove(c);

            Assert.IsTrue(removed);
            Assert.AreEqual(4, tree2D.Count);
        }

        [Test]
        public void Remove_AllNodes_LeavesEmptyTree()
        {
            object a = new();
            object b = new();
            object c = new();

            tree2D.Add(a, new Vector2(5, 5));
            tree2D.Add(b, new Vector2(3, 3));
            tree2D.Add(c, new Vector2(7, 7));

            tree2D.Remove(a);
            tree2D.Remove(b);
            tree2D.Remove(c);

            Assert.AreEqual(0, tree2D.Count);
        }
        
        [Test]
        public void Remove_TwoValuesSameNode_RemovesBothWithoutException()
        {
            tree2D.Add("A", Vector2.zero);
            tree2D.Add("B", Vector2.zero);

            Assert.DoesNotThrow(() =>
            {
                tree2D.Remove("A");
                tree2D.Remove("B");
            });

            Assert.AreEqual(0, tree2D.Count);
        }
        
        [Test]
        public void Remove_MultipleValuesSameNode_ReverseOrder()
        {
            tree2D.Add("A", Vector2.zero);
            tree2D.Add("B", Vector2.zero);
            tree2D.Add("C", Vector2.zero);

            Assert.DoesNotThrow(() =>
            {
                tree2D.Remove("C");
                tree2D.Remove("B");
                tree2D.Remove("A");
            });

            Assert.AreEqual(0, tree2D.Count);
        }
        
        [Test]
        public void Remove_AfterNodeReplacement_DoesNotThrow()
        {
            tree2D.Add("A", new Vector2(5, 5));
            tree2D.Add("B", new Vector2(3, 3));
            tree2D.Add("C", new Vector2(7, 7));
            tree2D.Add("D", new Vector2(6, 6));

            tree2D.Remove("A");

            Assert.DoesNotThrow(() =>
            {
                tree2D.Remove("C");
                tree2D.Remove("D");
                tree2D.Remove("B");
            });

            Assert.AreEqual(0, tree2D.Count);
        }
        
        [Test]
        public void Remove_1000SequentialValues_DoesNotThrow()
        {
            const int count = 1000;

            for (int i = 0; i < count; i++)
            {
                tree2D.Add(i, new Vector2(i, i));
            }

            Assert.DoesNotThrow(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    tree2D.Remove(i);
                }
            });

            Assert.AreEqual(0, tree2D.Count);
        }
        
        [Test]
        public void Remove_1000ValuesSamePoint_DoesNotThrow()
        {
            const int count = 1000;

            for (int i = 0; i < count; i++)
            {
                tree2D.Add(i, Vector2.zero);
            }

            Assert.DoesNotThrow(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    tree2D.Remove(i);
                }
            });

            Assert.AreEqual(0, tree2D.Count);
        }
        
        [Test]
        public void AddRemove_1000RandomValues_TreeBecomesEmpty()
        {
            const int count = 1000;

            for (int i = 0; i < count; i++)
            {
                tree2D.Add(
                    i,
                    new Vector2(
                        Random.Range(0f, 1000f),
                        Random.Range(0f, 1000f)
                    )
                );
            }

            for (int i = 0; i < count; i++)
            {
                bool removed = tree2D.Remove(i);

                Assert.IsTrue(removed, $"Failed to remove value {i}");
            }

            Assert.AreEqual(0, tree2D.Count);
            Assert.IsNull(tree2D.root);
        }
    }
}