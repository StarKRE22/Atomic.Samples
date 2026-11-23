using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class UniformGrid2DTests_QueryRadius
    {
        private UniformGrid2D<int> grid;
        private int[] buffer;

        [SetUp]
        public void Setup()
        {
            grid = new UniformGrid2D<int>(10, 10, 1f);
            buffer = new int[100];
        }

        [Test]
        public void Query_ShouldReturnZero_WhenEmpty()
        {
            int count = grid.QueryCircle(new Vector2(5, 5), 1f, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Query_ShouldFindSingleObject()
        {
            grid.Add(1, new Vector2(5, 5));

            int count = grid.QueryCircle(new Vector2(5, 5), 0.5f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldRespectRadius()
        {
            grid.Add(1, new Vector2(5, 5));
            grid.Add(2, new Vector2(7, 7));

            int count = grid.QueryCircle(new Vector2(5, 5), 1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldFindMultipleObjects()
        {
            grid.Add(1, new Vector2(5, 5));
            grid.Add(2, new Vector2(5.2f, 5.1f));
            grid.Add(3, new Vector2(4.9f, 5.3f));

            int count = grid.QueryCircle(new Vector2(5, 5), 1f, buffer);

            Assert.AreEqual(3, count);
        }

        [Test]
        public void Query_ShouldIgnoreObjectsOutsideCircleButInsideCells()
        {
            grid.Add(1, new Vector2(5, 5));
            grid.Add(2, new Vector2(5.9f, 5.9f));

            int count = grid.QueryCircle(new Vector2(5, 5), 0.5f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldRespectBufferLimit()
        {
            for (int i = 0; i < 10; i++)
                grid.Add(i, new Vector2(5 + i * 0.01f, 5));

            int smallBufferSize = 5;
            int[] smallBuffer = new int[smallBufferSize];

            int count = grid.QueryCircle(new Vector2(5, 5), 2f, smallBuffer);

            Assert.AreEqual(smallBufferSize, count);
        }

        [Test]
        public void Query_ShouldWorkNearBounds()
        {
            grid.Add(1, new Vector2(0.1f, 0.1f));

            int count = grid.QueryCircle(new Vector2(0, 0), 1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void Query_ShouldHandleManyObjects()
        {
            for (int i = 0; i < 1000; i++)
                grid.Add(i, new Vector2(i % 10, i % 10));

            int count = grid.QueryCircle(new Vector2(5, 5), 5f, buffer);

            Assert.Greater(count, 0);
        }

        [Test]
        public void Query_ShouldNotReturnDuplicates()
        {
            grid.Add(1, new Vector2(5, 5));

            int count = grid.QueryCircle(new Vector2(5, 5), 1f, buffer);

            Assert.AreEqual(1, count);
        }
    }
}