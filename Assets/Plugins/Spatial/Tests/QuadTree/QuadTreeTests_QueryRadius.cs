using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class QuadTreeTests_QueryRadius
    {
        private QuadTree<int> tree;

        [SetUp]
        public void Setup()
        {
            tree = new QuadTree<int>(
                new Vector2(0f, 0f),
                new Vector2(10f, 10f),
                2,
                4
            );

            tree.Add(1, new Vector2(1f, 1f));
            tree.Add(2, new Vector2(5f, 5f));
            tree.Add(3, new Vector2(8f, 5f));
            tree.Add(4, new Vector2(9f, 9f));
        }

        [Test]
        public void QueryRadius_AllInside_ReturnsAll()
        {
            int[] results = new int[10];

            int count = tree.QueryCircle(
                new Vector2(5f, 5f),
                10f,
                results
            );

            Assert.AreEqual(4, count);

            CollectionAssert.AreEquivalent(
                new[] {1, 2, 3, 4},
                results.Take(count)
            );
        }

        [Test]
        public void QueryRadius_SmallRadius_ReturnsCenterOnly()
        {
            int[] results = new int[10];

            int count = tree.QueryCircle(
                new Vector2(5f, 5f),
                0.1f,
                results
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(2, results[0]);
        }

        [Test]
        public void QueryRadius_ExactBoundary_Included()
        {
            int[] results = new int[10];

            int count = tree.QueryCircle(
                new Vector2(5f, 5f),
                3f,
                results
            );

            Assert.AreEqual(2, count);

            CollectionAssert.Contains(
                results.Take(count).ToArray(),
                3
            );
        }

        [Test]
        public void QueryRadius_NoMatches_ReturnsZero()
        {
            int[] results = new int[10];

            int count = tree.QueryCircle(
                new Vector2(0f, 0f),
                0.5f,
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRadius_OutsidePoints_Excluded()
        {
            int[] results = new int[10];

            int count = tree.QueryCircle(
                new Vector2(5f, 5f),
                2f,
                results
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(2, results[0]);
        }

        [Test]
        public void QueryRadius_EmptyTree_ReturnsZero()
        {
            tree.Clear();

            int[] results = new int[10];

            int count = tree.QueryCircle(
                Vector2.zero,
                100f,
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRadius_ResultBufferTooSmall_ReturnsBufferSize()
        {
            int[] results = new int[1];

            int count = tree.QueryCircle(
                new Vector2(5f, 5f),
                10f,
                results
            );

            Assert.That(count, Is.EqualTo(results.Length));
        }
    }
}