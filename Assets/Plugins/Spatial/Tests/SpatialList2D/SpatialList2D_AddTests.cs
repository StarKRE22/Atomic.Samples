using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed class SpatialList2D_AddTests
    {
        [Test]
        public void Add_FirstItem_ReturnsTrue()
        {
            var list = new SpatialList2D<int>();

            bool result = list.Add(42, new Vector2(1, 2));

            Assert.That(result, Is.True);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list.Contains(42), Is.True);
        }

        [Test]
        public void Add_DuplicateItem_ReturnsFalse()
        {
            var list = new SpatialList2D<int>();

            list.Add(42, Vector2.zero);

            bool result = list.Add(42, Vector2.one);

            Assert.That(result, Is.False);
            Assert.That(list.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_MultipleItems_IncreasesCount()
        {
            var list = new SpatialList2D<int>();

            list.Add(1, Vector2.zero);
            list.Add(2, Vector2.zero);
            list.Add(3, Vector2.zero);

            Assert.That(list.Count, Is.EqualTo(3));

            Assert.That(list.Contains(1), Is.True);
            Assert.That(list.Contains(2), Is.True);
            Assert.That(list.Contains(3), Is.True);
        }

        [Test]
        public void Add_MoreThanInitialCapacity_ResizesArray()
        {
            var list = new SpatialList2D<int>();

            for (int i = 0; i < 10; i++)
                Assert.That(list.Add(i, Vector2.zero), Is.True);

            Assert.That(list.Count, Is.EqualTo(10));

            for (int i = 0; i < 10; i++)
                Assert.That(list.Contains(i), Is.True);
        }
    }
}
