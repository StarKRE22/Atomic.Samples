using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreeTests_QueryRect
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
            tree.Add(3, new Vector2(8f, 5f));
            tree.Add(4, new Vector2(9f, 9f));
        }

        [Test]
        public void QueryRect_AllItems_ReturnsAll()
        {
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
        public void QueryRect_SingleItem_ReturnsOnlyMatchingItem()
        {
            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(4f, 4f, 2f, 2f),
                results
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(2, results[0]);
        }

        [Test]
        public void QueryRect_NoMatches_ReturnsZero()
        {
            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(2f, 7f, 1f, 1f),
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_PartialArea_ReturnsMatchingItems()
        {
            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(4f, 4f, 6f, 2f),
                results
            );

            Assert.AreEqual(2, count);

            CollectionAssert.AreEquivalent(
                new[] { 2, 3 },
                results[..count]
            );
        }

        [Test]
        public void QueryRect_ExactBoundary_IncludesPoint()
        {
            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(8f, 5f, 0f, 0f),
                results
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(3, results[0]);
        }

        [Test]
        public void QueryRect_EmptyTree_ReturnsZero()
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
        public void QueryRect_BufferSmallerThanResult_TruncatesOutput()
        {
            int[] results = new int[2];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            Assert.AreEqual(2, count);
        }

        [Test]
        public void QueryRect_OutsideTreeBounds_ReturnsZero()
        {
            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(100f, 100f, 10f, 10f),
                results
            );

            Assert.AreEqual(0, count);
        }
    }
}