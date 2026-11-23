using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public sealed class UniformGrid2DTests_QueryRect
    {
        private UniformGrid2D<string> grid;

        [SetUp]
        public void SetUp()
        {
            grid = new UniformGrid2D<string>(
                sizeX: 20,
                sizeY: 20,
                cellSize: 1f
            );
        }

        [Test]
        public void QueryRect_EmptyGrid_ReturnsZero()
        {
            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(0, 0, 10, 10),
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_SingleItemInside_ReturnsOne()
        {
            grid.Add("A", new Vector2(5, 5));

            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(4, 4, 2, 2),
                results
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", results[0]);
        }

        [Test]
        public void QueryRect_SingleItemOutside_ReturnsZero()
        {
            grid.Add("A", new Vector2(5, 5));

            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(0, 0, 2, 2),
                results
            );

            Assert.AreEqual(0, count);
        }

        [Test]
        public void QueryRect_MultipleItems_ReturnsAllInside()
        {
            grid.Add("A", new Vector2(2, 2));
            grid.Add("B", new Vector2(4, 4));
            grid.Add("C", new Vector2(6, 6));
            grid.Add("D", new Vector2(8, 8));

            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(1, 1, 6, 6),
                results
            );

            Assert.AreEqual(3, count);

            CollectionAssert.Contains(results, "A");
            CollectionAssert.Contains(results, "B");
            CollectionAssert.Contains(results, "C");
        }

        [Test]
        public void QueryRect_MultipleCells_ReturnsCorrectItems()
        {
            grid.Add("A", new Vector2(1, 1));
            grid.Add("B", new Vector2(5, 5));
            grid.Add("C", new Vector2(9, 9));
            grid.Add("D", new Vector2(15, 15));

            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(0, 0, 10, 10),
                results
            );

            Assert.AreEqual(3, count);

            CollectionAssert.Contains(results, "A");
            CollectionAssert.Contains(results, "B");
            CollectionAssert.Contains(results, "C");
        }

        [Test]
        public void QueryRect_RectPartiallyOutsideGrid_ReturnsValidItems()
        {
            grid.Add("A", new Vector2(1, 1));
            grid.Add("B", new Vector2(5, 5));

            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(-100, -100, 200, 200),
                results
            );

            Assert.AreEqual(2, count);

            CollectionAssert.Contains(results, "A");
            CollectionAssert.Contains(results, "B");
        }

        [Test]
        public void QueryRect_BufferTooSmall_StopsAtCapacity()
        {
            grid.Add("A", new Vector2(1, 1));
            grid.Add("B", new Vector2(2, 2));
            grid.Add("C", new Vector2(3, 3));

            string[] results = new string[2];

            int count = grid.QueryRect(
                new Rect(0, 0, 10, 10),
                results
            );

            Assert.AreEqual(2, count);
        }

        [Test]
        public void QueryRect_MultipleItemsInSameCell_ReturnsAll()
        {
            grid.Add("A", new Vector2(5.1f, 5.1f));
            grid.Add("B", new Vector2(5.2f, 5.2f));
            grid.Add("C", new Vector2(5.3f, 5.3f));

            string[] results = new string[10];

            int count = grid.QueryRect(
                new Rect(5, 5, 1, 1),
                results
            );

            Assert.AreEqual(3, count);

            CollectionAssert.Contains(results, "A");
            CollectionAssert.Contains(results, "B");
            CollectionAssert.Contains(results, "C");
        }
    }
}