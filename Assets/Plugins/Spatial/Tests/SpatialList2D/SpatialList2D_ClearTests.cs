using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_ClearTests
    {
        [Test]
        public void Clear_EmptyContainer_RemainsEmpty()
        {
            var list = new SpatialList2D<int>();

            list.Clear();

            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);
            list.Add(3, Vector2.right);

            list.Clear();

            Assert.That(list.Count, Is.EqualTo(0));

            Assert.That(list.Contains(1), Is.False);
            Assert.That(list.Contains(2), Is.False);
            Assert.That(list.Contains(3), Is.False);
        }

        [Test]
        public void Clear_QueryClosest_ReturnsFalse()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);

            list.Clear();

            bool found = list.QueryClosest(
                Vector2.zero,
                out _,
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void Clear_QueryCircle_ReturnsZero()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);

            list.Clear();

            int[] results = new int[4];

            int count = list.QueryCircle(
                Vector2.zero,
                100f,
                results);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void Clear_QueryRect_ReturnsZero()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);

            list.Clear();

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(-10, -10, 20, 20),
                results);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void Clear_AllowsAddingItemsAgain()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);

            list.Clear();

            bool added = list.Add(42, new Vector2(10, 20));

            Assert.That(added, Is.True);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list.Contains(42), Is.True);
        }

        [Test]
        public void Clear_AllowsReAddingSameItem()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, Vector2.zero);

            list.Clear();

            bool added = list.Add(42, Vector2.one);

            Assert.That(added, Is.True);
            Assert.That(list.Count, Is.EqualTo(1));
        }
    }
}