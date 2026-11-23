using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class QuadTreeTests_Clear
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
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            tree.Clear();

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Clear_QueryClosest_ReturnsFalse()
        {
            tree.Clear();

            bool found = tree.QueryClosest(
                Vector2.zero,
                out _,
                out int item
            );

            Assert.IsFalse(found);
            Assert.AreEqual(0, item);
        }

        [Test]
        public void Clear_Remove_ReturnsFalse()
        {
            tree.Clear();

            Assert.IsFalse(
                tree.Remove(1)
            );
        }

        [Test]
        public void Clear_Move_ReturnsFalse()
        {
            tree.Clear();

            Assert.IsFalse(
                tree.Move(1, new Vector2(5f, 5f))
            );
        }

        [Test]
        public void Clear_AllowsInsertAgain()
        {
            tree.Clear();

            bool inserted = tree.Add(
                100,
                new Vector2(3f, 3f)
            );

            Assert.IsTrue(inserted);

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(100, results[0]);
        }

        [Test]
        public void Clear_PreventsDuplicateDetectionFromOldItems()
        {
            tree.Clear();

            Assert.IsTrue(
                tree.Add(1, new Vector2(2f, 2f))
            );
        }

        [Test]
        public void Clear_CanBeCalledMultipleTimes()
        {
            tree.Clear();
            tree.Clear();
            tree.Clear();

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(0, count);
        }
    }
}