using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_QueryClosestTests
    {
        [Test]
        public void QueryClosest_EmptyContainer_ReturnsFalse()
        {
            var list = new SpatialList2D<int>();

            bool found = list.QueryClosest(
                Vector2.zero,
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.False);
            Assert.That(closestPoint, Is.EqualTo(default(Vector2)));
            Assert.That(closest, Is.EqualTo(0));
        }

        [Test]
        public void QueryClosest_SingleItem_ReturnsItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, new Vector2(10, 20));

            bool found = list.QueryClosest(
                Vector2.zero,
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.True);
            Assert.That(closest, Is.EqualTo(42));
            Assert.That(closestPoint, Is.EqualTo(new Vector2(10, 20)));
        }

        [Test]
        public void QueryClosest_ReturnsNearestItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(100, 100));
            list.Add(2, new Vector2(5, 5));
            list.Add(3, new Vector2(20, 20));

            bool found = list.QueryClosest(
                Vector2.zero,
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.True);
            Assert.That(closest, Is.EqualTo(2));
            Assert.That(closestPoint, Is.EqualTo(new Vector2(5, 5)));
        }

        [Test]
        public void QueryClosest_ReturnsNearestAmongMany()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(0, 0));
            list.Add(2, new Vector2(10, 0));
            list.Add(3, new Vector2(20, 0));

            bool found = list.QueryClosest(
                new Vector2(12, 0),
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.True);
            Assert.That(closest, Is.EqualTo(2));
            Assert.That(closestPoint, Is.EqualTo(new Vector2(10, 0)));
        }

        [Test]
        public void QueryClosest_AfterMove_ReturnsUpdatedPosition()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, new Vector2(100, 100));

            list.Move(1, new Vector2(200, 200));

            bool found = list.QueryClosest(
                new Vector2(90, 90),
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.True);
            Assert.That(closest, Is.EqualTo(2));
            Assert.That(closestPoint, Is.EqualTo(new Vector2(100, 100)));
        }

        [Test]
        public void QueryClosest_AfterRemove_IgnoresRemovedItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, new Vector2(10, 0));

            list.Remove(1);

            bool found = list.QueryClosest(
                Vector2.zero,
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.True);
            Assert.That(closest, Is.EqualTo(2));
            Assert.That(closestPoint, Is.EqualTo(new Vector2(10, 0)));
        }
    }
}