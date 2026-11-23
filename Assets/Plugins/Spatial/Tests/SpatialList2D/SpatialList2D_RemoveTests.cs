using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_RemoveTests
    {
        [Test]
        public void Remove_MissingItem_ReturnsFalse()
        {
            var list = new SpatialList2D<int>();

            bool result = list.Remove(42);

            Assert.That(result, Is.False);
            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ExistingItem_ReturnsTrue()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, Vector2.zero);

            bool result = list.Remove(42);

            Assert.That(result, Is.True);
            Assert.That(list.Count, Is.EqualTo(0));
            Assert.That(list.Contains(42), Is.False);
        }

        [Test]
        public void Remove_OnlyItem_ContainerBecomesEmpty()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);

            list.Remove(1);

            Assert.That(list.Count, Is.EqualTo(0));

            bool found = list.QueryClosest(
                Vector2.zero,
                out _,
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void Remove_FirstItem_PreservesRemainingItems()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);
            list.Add(3, Vector2.right);

            list.Remove(1);

            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list.Contains(1), Is.False);
            Assert.That(list.Contains(2), Is.True);
            Assert.That(list.Contains(3), Is.True);
        }

        [Test]
        public void Remove_LastItem_PreservesRemainingItems()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);
            list.Add(3, Vector2.right);

            list.Remove(3);

            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list.Contains(1), Is.True);
            Assert.That(list.Contains(2), Is.True);
            Assert.That(list.Contains(3), Is.False);
        }

        [Test]
        public void Remove_MiddleItem_PreservesRemainingItems()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);
            list.Add(3, Vector2.right);

            list.Remove(2);

            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list.Contains(1), Is.True);
            Assert.That(list.Contains(2), Is.False);
            Assert.That(list.Contains(3), Is.True);
        }

        [Test]
        public void Remove_SwapBack_UpdatesLookup()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, new Vector2(0, 0));
            list.Add(2, new Vector2(10, 0));
            list.Add(3, new Vector2(20, 0));

            list.Remove(1);

            bool moved = list.Move(3, new Vector2(100, 100));

            Assert.That(moved, Is.True);

            int[] results = new int[4];

            int count = list.QueryRect(
                new Rect(90, 90, 20, 20),
                results);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(3));
        }

        [Test]
        public void Remove_AllItems_ContainerBecomesEmpty()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.one);
            list.Add(3, Vector2.right);

            list.Remove(1);
            list.Remove(2);
            list.Remove(3);

            Assert.That(list.Count, Is.EqualTo(0));

            bool found = list.QueryClosest(
                Vector2.zero,
                out _,
                out _);

            Assert.That(found, Is.False);
        }
    }
}