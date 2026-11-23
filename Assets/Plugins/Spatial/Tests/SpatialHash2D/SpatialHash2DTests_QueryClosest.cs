using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class SpatialHash2DTests_QueryClosest
    {
        private SpatialHash2D<int> hash;

        [SetUp]
        public void Setup()
        {
            hash = new SpatialHash2D<int>(cellSize: 1f);
        }

        [Test]
        public void QueryClosest_ShouldReturnFalse_WhenEmpty()
        {
            bool found = hash.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int value);

            Assert.IsFalse(found);
            Assert.AreEqual(0, value);
            Assert.AreEqual(default(Vector2), point);
        }

        [Test]
        public void QueryClosest_ShouldFindSingleObject()
        {
            hash.Add(1, new Vector2(5, 5));

            bool found = hash.QueryClosest(
                new Vector2(0, 0),
                out Vector2 point,
                out int value);

            Assert.IsTrue(found);
            Assert.AreEqual(1, value);
            Assert.AreEqual(new Vector2(5, 5), point);
        }

        [Test]
        public void QueryClosest_ShouldReturnNearestObject()
        {
            hash.Add(1, new Vector2(5, 5));
            hash.Add(2, new Vector2(1, 1));
            hash.Add(3, new Vector2(10, 10));

            bool found = hash.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int value);

            Assert.IsTrue(found);
            Assert.AreEqual(2, value);
            Assert.AreEqual(new Vector2(1, 1), point);
        }

        [Test]
        public void QueryClosest_ShouldWorkWithNegativeCoordinates()
        {
            hash.Add(1, new Vector2(-1, -1));
            hash.Add(2, new Vector2(5, 5));

            bool found = hash.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int value);

            Assert.IsTrue(found);
            Assert.AreEqual(1, value);
            Assert.AreEqual(new Vector2(-1, -1), point);
        }

        [Test]
        public void QueryClosest_ShouldFindObjectInAnotherCell()
        {
            hash.Add(1, new Vector2(10, 10));

            bool found = hash.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int value);

            Assert.IsTrue(found);
            Assert.AreEqual(1, value);
            Assert.AreEqual(new Vector2(10, 10), point);
        }

        [Test]
        public void QueryClosest_ShouldReturnExactMatch_WhenCenterMatchesObject()
        {
            hash.Add(1, new Vector2(5, 5));
            hash.Add(2, new Vector2(5.1f, 5.1f));

            bool found = hash.QueryClosest(
                new Vector2(5, 5),
                out Vector2 point,
                out int value);

            Assert.IsTrue(found);
            Assert.AreEqual(1, value);
            Assert.AreEqual(new Vector2(5, 5), point);
        }
    }
}