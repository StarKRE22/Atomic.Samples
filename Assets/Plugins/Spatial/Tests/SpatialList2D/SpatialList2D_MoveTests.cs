using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_MoveTests
    {
        [Test]
        public void Move_MissingItem_ReturnsFalse()
        {
            var list = new SpatialList2D<int>();

            bool result = list.Move(42, new Vector2(10, 20));

            Assert.That(result, Is.False);
            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void Move_ExistingItem_ReturnsTrue()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, Vector2.zero);

            bool result = list.Move(42, new Vector2(10, 20));

            Assert.That(result, Is.True);
            Assert.That(list.Count, Is.EqualTo(1));
        }

        [Test]
        public void Move_UpdatesPositionForQueryRect()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, Vector2.zero);

            list.Move(42, new Vector2(100, 100));

            int[] results = new int[4];

            int countOld = list.QueryRect(
                new Rect(-1, -1, 2, 2),
                results);

            int countNew = list.QueryRect(
                new Rect(99, 99, 2, 2),
                results);

            Assert.That(countOld, Is.EqualTo(0));
            Assert.That(countNew, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(42));
        }

        [Test]
        public void Move_UpdatesPositionForQueryClosest()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, new Vector2(100, 100));

            list.Move(1, new Vector2(200, 200));

            bool found = list.QueryClosest(
                new Vector2(110, 110),
                out Vector2 closestPoint,
                out int closest);

            Assert.That(found, Is.True);
            Assert.That(closest, Is.EqualTo(2));
            Assert.That(closestPoint, Is.EqualTo(new Vector2(100, 100)));
        }

        [Test]
        public void Move_DoesNotChangeCount()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);

            int before = list.Count;

            list.Move(1, Vector2.one);

            Assert.That(list.Count, Is.EqualTo(before));
        }
    }
}