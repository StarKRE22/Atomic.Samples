using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_QueryRectTests
    {
        [Test]
        public void QueryRect_EmptyContainer_ReturnsZero()
        {
            var list = new SpatialList2D<int>();

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(0, 0, 10, 10),
                results);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void QueryRect_NoMatches_ReturnsZero()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(100, 100));

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(0, 0, 10, 10),
                results);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void QueryRect_SingleMatch_ReturnsItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, new Vector2(5, 5));

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(0, 0, 10, 10),
                results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(42));
        }

        [Test]
        public void QueryRect_MultipleMatches_ReturnsAllItems()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(1, 1));
            list.Add(2, new Vector2(5, 5));
            list.Add(3, new Vector2(9, 9));
            list.Add(4, new Vector2(20, 20));

            int[] results = new int[8];

            int count = list.QueryRect(
                new Rect(0, 0, 10, 10),
                results);

            Assert.That(count, Is.EqualTo(3));

            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                results[..count]);
        }

        [Test]
        public void QueryRect_PointOnMinBoundary_IsIncluded()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(0, 0));

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(0, 0, 10, 10),
                results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(1));
        }

        [Test]
        public void QueryRect_PointOnMaxBoundary_IsIncluded()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(10, 10));

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(0, 0, 10, 10),
                results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(1));
        }

        [Test]
        public void QueryRect_ResultBufferTooSmall_IsClamped()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);
            list.Add(3, new Vector2(2, 2));

            int[] results = new int[2];

            int count = list.QueryRect(
                new Rect(-10, -10, 20, 20),
                results);

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void QueryRect_AfterMove_UsesUpdatedPosition()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);

            list.Move(1, new Vector2(100, 100));

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(90, 90, 20, 20),
                results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(1));
        }

        [Test]
        public void QueryRect_AfterRemove_DoesNotReturnRemovedItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Remove(1);

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(-1, -1, 2, 2),
                results);

            Assert.That(count, Is.EqualTo(0));
        }
    }
}