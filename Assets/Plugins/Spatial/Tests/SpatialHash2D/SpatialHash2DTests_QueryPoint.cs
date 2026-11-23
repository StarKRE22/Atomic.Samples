using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class SpatialHash2DTests_QueryPoint
    {
        private SpatialHash2D<int> hash;
        private int[] buffer;

        [SetUp]
        public void Setup()
        {
            hash = new SpatialHash2D<int>(cellSize: 1f);
            buffer = new int[100];
        }

        [Test]
        public void Query_ShouldReturnZero_WhenEmpty()
        {
            int count = hash.QueryPoint(Vector2.zero, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Query_ShouldFindSingleObject()
        {
            hash.Add(1, new Vector2(5, 5));

            int count = hash.QueryPoint(new Vector2(5, 5), buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldReturnZero_WhenPointDoesNotExist()
        {
            hash.Add(1, new Vector2(5, 5));

            int count = hash.QueryPoint(new Vector2(5.1f, 5f), buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Query_ShouldRequireExactPositionMatch()
        {
            hash.Add(1, new Vector2(1f, 1f));

            int count = hash.QueryPoint(
                new Vector2(1.0001f, 1f),
                buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Query_ShouldReturnMultipleObjectsAtSamePosition()
        {
            hash.Add(1, new Vector2(5, 5));
            hash.Add(2, new Vector2(5, 5));
            hash.Add(3, new Vector2(5, 5));

            int count = hash.QueryPoint(new Vector2(5, 5), buffer);

            Assert.AreEqual(3, count);
        }

        [Test]
        public void Query_ShouldIgnoreObjectsInSameCellButDifferentPosition()
        {
            hash.Add(1, new Vector2(5.1f, 5.1f));
            hash.Add(2, new Vector2(5.2f, 5.2f));

            int count = hash.QueryPoint(new Vector2(5.1f, 5.1f), buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldRespectBufferLimit()
        {
            for (int i = 0; i < 10; i++)
                hash.Add(i, new Vector2(5, 5));

            int[] smallBuffer = new int[5];

            int count = hash.QueryPoint(
                new Vector2(5, 5),
                smallBuffer);

            Assert.AreEqual(5, count);
        }

        [Test]
        public void Query_ShouldWorkWithNegativeCoordinates()
        {
            hash.Add(1, new Vector2(-5, -5));

            int count = hash.QueryPoint(
                new Vector2(-5, -5),
                buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldFindObjectsOnCellBoundary()
        {
            hash.Add(1, new Vector2(1f, 1f));

            int count = hash.QueryPoint(
                new Vector2(1f, 1f),
                buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldNotReturnDuplicates()
        {
            hash.Add(1, new Vector2(5, 5));

            int count = hash.QueryPoint(
                new Vector2(5, 5),
                buffer);

            Assert.AreEqual(1, count);
        }
    }
}