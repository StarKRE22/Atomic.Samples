using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace Modules.Spatial
{
    [TestFixture]
    public class NativeSpatialHash2DTests_QueryClosest
    {
        private NativeSpatialHash2D<int> _hash;

        [SetUp]
        public void Setup()
        {
            _hash = new NativeSpatialHash2D<int>(
                Allocator.Temp,
                cellSize: 1f);
        }

        [TearDown]
        public void TearDown()
        {
            _hash.Dispose();
        }

        [Test]
        public void QueryClosest_EmptyHash_ReturnsFalse()
        {
            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                10f,
                out _,
                out _);

            Assert.IsFalse(found);
        }

        [Test]
        public void QueryClosest_SingleElement_ReturnsElement()
        {
            _hash.Add(1, new float2(2, 3));

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                10f,
                out float2 point,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(1, item);
            Assert.AreEqual(new float2(2, 3), point);
        }

        [Test]
        public void QueryClosest_ElementOutsideRadius_ReturnsFalse()
        {
            _hash.Add(1, new float2(100, 100));

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                10f,
                out _,
                out _);

            Assert.IsFalse(found);
        }

        [Test]
        public void QueryClosest_ReturnsNearestElement()
        {
            _hash.Add(1, new float2(10, 0));
            _hash.Add(2, new float2(3, 0));
            _hash.Add(3, new float2(5, 0));

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                20f,
                out float2 point,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(2, item);
            Assert.AreEqual(new float2(3, 0), point);
        }

        [Test]
        public void QueryClosest_UsesActualDistance_NotCellDistance()
        {
            _hash.Add(1, new float2(0.1f, 0.1f));
            _hash.Add(2, new float2(1.1f, 1.1f));

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                10f,
                out _,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(1, item);
        }

        [Test]
        public void QueryClosest_NegativeCoordinates_Works()
        {
            _hash.Add(1, new float2(-2, -2));
            _hash.Add(2, new float2(-10, -10));

            bool found = _hash.QueryClosestInRadius(
                new float2(-1, -1),
                20f,
                out _,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(1, item);
        }

        [Test]
        public void QueryClosest_AfterMove_ReturnsNewPosition()
        {
            _hash.Add(1, new float2(0, 0));

            _hash.Move(1, new float2(10, 10));

            bool found = _hash.QueryClosestInRadius(
                new float2(10, 10),
                1f,
                out float2 point,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(1, item);
            Assert.AreEqual(new float2(10, 10), point);
        }

        [Test]
        public void QueryClosest_AfterRemove_ReturnsFalse()
        {
            _hash.Add(1, new float2(0, 0));

            _hash.Remove(1);

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                10f,
                out _,
                out _);

            Assert.IsFalse(found);
        }

        [Test]
        public void QueryClosest_OnlyRemainingItemReturnedAfterRemove()
        {
            _hash.Add(1, new float2(1, 0));
            _hash.Add(2, new float2(2, 0));

            _hash.Remove(1);

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                10f,
                out _,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(2, item);
        }

        [Test]
        public void QueryClosest_SearchRadiusLimitsResults()
        {
            _hash.Add(1, new float2(5, 0));

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                4f,
                out _,
                out _);

            Assert.IsFalse(found);
        }
        
        [Test]
        public void QueryClosest_OnRadiusBoundary_ReturnsElement()
        {
            _hash.Add(1, new float2(5, 0));

            bool found = _hash.QueryClosestInRadius(
                float2.zero,
                5f,
                out _,
                out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(1, item);
        }
    }
}