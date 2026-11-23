using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class SpatialHash2DTests_QueryRect
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
        public void QueryRect_ShouldReturnZero_WhenEmpty()
        {
            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_ShouldFindSingleObject()
        {
            hash.Add(1, new Vector2(5, 5));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void QueryRect_ShouldFindMultipleObjects()
        {
            hash.Add(1, new Vector2(1, 1));
            hash.Add(2, new Vector2(2, 2));
            hash.Add(3, new Vector2(3, 3));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(3, count);
        }

        [Test]
        public void QueryRect_ShouldIgnoreObjectsOutsideRect()
        {
            hash.Add(1, new Vector2(5, 5));
            hash.Add(2, new Vector2(15, 15));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void QueryRect_ShouldRespectBufferLimit()
        {
            for (int i = 0; i < 10; i++)
                hash.Add(i, new Vector2(1 + i * 0.1f, 1));

            int[] smallBuffer = new int[5];

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                smallBuffer);

            Assert.AreEqual(5, count);
        }

        [Test]
        public void QueryRect_ShouldWorkWithNegativeCoordinates()
        {
            hash.Add(1, new Vector2(-5, -5));

            int count = hash.QueryRect(
                new Rect(-10, -10, 10, 10),
                buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void QueryRect_ShouldFindObjectsAcrossMultipleCells()
        {
            hash.Add(1, new Vector2(0.1f, 0.1f));
            hash.Add(2, new Vector2(2.1f, 2.1f));
            hash.Add(3, new Vector2(4.1f, 4.1f));

            int count = hash.QueryRect(
                new Rect(0, 0, 5, 5),
                buffer);

            Assert.AreEqual(3, count);
        }

        [Test]
        public void QueryRect_ShouldIncludePointOnMinBoundary()
        {
            hash.Add(1, new Vector2(0, 0));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void QueryRect_ShouldExcludePointOnMaxXBoundary()
        {
            hash.Add(1, new Vector2(10, 5));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_ShouldExcludePointOnMaxYBoundary()
        {
            hash.Add(1, new Vector2(5, 10));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_ShouldExcludePointOnMaxCorner()
        {
            hash.Add(1, new Vector2(10, 10));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_ShouldNotReturnDuplicates()
        {
            hash.Add(1, new Vector2(5, 5));

            int count = hash.QueryRect(
                new Rect(0, 0, 10, 10),
                buffer);

            Assert.AreEqual(1, count);
        }
    }
}