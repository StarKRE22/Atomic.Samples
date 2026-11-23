using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreeTests_Add
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
        }

        [Test]
        public void Add_Inside_ReturnsTrue()
        {
            Assert.IsTrue(
                tree.Add(1, new Vector2(5f, 5f))
            );
        }

        [Test]
        public void Add_Outside_ReturnsFalse()
        {
            Assert.IsFalse(
                tree.Add(1, new Vector2(-1f, 5f))
            );
        }

        [Test]
        public void Add_DuplicateItem_ReturnsFalse()
        {
            Assert.IsTrue(
                tree.Add(1, new Vector2(5f, 5f))
            );

            Assert.IsFalse(
                tree.Add(1, new Vector2(6f, 6f))
            );
        }

        [Test]
        public void Add_FillsCapacity_NoErrors()
        {
            Assert.IsTrue(
                tree.Add(1, new Vector2(2f, 2f))
            );

            Assert.IsTrue(
                tree.Add(2, new Vector2(3f, 3f))
            );
        }

        [Test]
        public void Add_TriggersSubdivision_ItemsRemainQueryable()
        {
            tree.Add(1, new Vector2(2f, 2f));
            tree.Add(2, new Vector2(3f, 3f));
            tree.Add(3, new Vector2(7f, 7f));

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(3, count);

            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                results[..count]
            );
        }

        [Test]
        public void Add_MaxDepthReached_StillStoresItems()
        {
            var smallTree = new QuadTree<int>(
                new Vector2(0f, 0f),
                new Vector2(10f, 10f),
                capacity: 1,
                maxDepth: 0
            );

            Assert.IsTrue(
                smallTree.Add(1, new Vector2(2f, 2f))
            );

            Assert.IsTrue(
                smallTree.Add(2, new Vector2(3f, 3f))
            );

            int[] results = new int[10];

            int count = smallTree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(2, count);
        }

        [Test]
        public void Add_ItemsInDifferentQuadrants_AreFound()
        {
            tree.Add(1, new Vector2(1f, 1f));
            tree.Add(2, new Vector2(9f, 1f));
            tree.Add(3, new Vector2(1f, 9f));
            tree.Add(4, new Vector2(9f, 9f));

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(4, count);

            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3, 4 },
                results[..count]
            );
        }

        [Test]
        public void Add_DuplicatePositions_AllStored()
        {
            tree.Add(1, new Vector2(5f, 5f));
            tree.Add(2, new Vector2(5f, 5f));

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(5f, 5f, 0f, 0f),
                results
            );

            Assert.AreEqual(2, count);

            CollectionAssert.AreEquivalent(
                new[] { 1, 2 },
                results[..count]
            );
        }
    }
}