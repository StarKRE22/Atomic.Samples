using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_QueryCircleTests
    {
        [Test]
        public void QueryCircle_EmptyContainer_ReturnsZero()
        {
            var list = new SpatialList2D<int>();

            int[] results = new int[4];

            int count = list.QueryCircle(Vector2.zero, 10f, results);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void QueryCircle_SingleMatch_ReturnsItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);

            int[] results = new int[4];

            int count = list.QueryCircle(Vector2.zero, 1f, results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(1));
        }

        [Test]
        public void QueryCircle_NoMatches_ReturnsZero()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(100, 100));

            int[] results = new int[4];

            int count = list.QueryCircle(Vector2.zero, 1f, results);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void QueryCircle_MultipleMatches_ReturnsAllItems()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, new Vector2(1, 1));
            list.Add(3, new Vector2(2, 2));
            list.Add(4, new Vector2(100, 100));

            int[] results = new int[8];

            int count = list.QueryCircle(Vector2.zero, 3f, results);

            Assert.That(count, Is.EqualTo(3));

            CollectionAssert.AreEquivalent(
                new[] {1, 2, 3},
                results[..count]);
        }

        [Test]
        public void QueryCircle_PointOnRadiusBoundary_IsIncluded()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(3, 4));

            int[] results = new int[4];

            int count = list.QueryCircle(Vector2.zero, 5f, results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(1));
        }

        [Test]
        public void QueryCircle_ResultBufferTooSmall_IsClamped()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.zero);
            list.Add(3, Vector2.zero);

            int[] results = new int[2];

            int count = list.QueryCircle(Vector2.zero, 1f, results);

            Assert.That(count, Is.EqualTo(2));
        }
    }
}