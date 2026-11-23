using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreeTests_Remove
    {
        private QuadTree<int> tree;

        [SetUp]
        public void Setup()
        {
            tree = new QuadTree<int>(
                new Vector2(0f, 0f),
                new Vector2(10f, 10f),
                capacity: 2,
                maxDepth: 4
            );

            tree.Add(1, new Vector2(1f, 1f));
            tree.Add(2, new Vector2(5f, 5f));
            tree.Add(3, new Vector2(8f, 8f));
            tree.Add(4, new Vector2(2f, 8f));
            tree.Add(5, new Vector2(8f, 2f));
        }

        [Test]
        public void Remove_ExistingItem_ReturnsTrue()
        {
            Assert.IsTrue(
                tree.Remove(3)
            );
        }

        [Test]
        public void Remove_MissingItem_ReturnsFalse()
        {
            Assert.IsFalse(
                tree.Remove(999)
            );
        }

        [Test]
        public void Remove_ItemIsNoLongerFound()
        {
            Assert.IsTrue(
                tree.Remove(3)
            );

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            CollectionAssert.DoesNotContain(
                results[..count],
                3
            );
        }

        [Test]
        public void Remove_OnlySpecifiedItemIsRemoved()
        {
            tree.Remove(3);

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 4, 5 },
                results[..count]
            );
        }

        [Test]
        public void Remove_AllItems_LeavesTreeEmpty()
        {
            tree.Remove(1);
            tree.Remove(2);
            tree.Remove(3);
            tree.Remove(4);
            tree.Remove(5);

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Remove_Twice_SecondCallReturnsFalse()
        {
            Assert.IsTrue(tree.Remove(3));
            Assert.IsFalse(tree.Remove(3));
        }

        [Test]
        public void Remove_DoesNotBreakQueryClosest()
        {
            tree.Remove(3);

            bool found = tree.QueryClosest(
                new Vector2(8f, 8f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreNotEqual(3, item);
        }

        [Test]
        public void Remove_LastItemInNode_WorksCorrectly()
        {
            Assert.IsTrue(
                tree.Remove(5)
            );

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            CollectionAssert.DoesNotContain(
                results[..count],
                5
            );
        }

        [Test]
        public void Remove_SwappedItem_RemainsAccessible()
        {
            tree.Remove(4);

            bool found = tree.QueryClosest(
                new Vector2(8f, 2f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(5, item);
        }

        [Test]
        public void Remove_CausesMerge_AllRemainingItemsStillQueryable()
        {
            tree.Remove(4);
            tree.Remove(5);
            tree.Remove(3);

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            CollectionAssert.AreEquivalent(
                new[] { 1, 2 },
                results[..count]
            );
        }
    }
}