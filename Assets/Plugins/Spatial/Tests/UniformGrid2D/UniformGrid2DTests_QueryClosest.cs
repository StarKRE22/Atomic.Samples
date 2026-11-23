using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public sealed class UniformGrid2DTests_QueryClosest
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
        public void QueryClosest_EmptyGrid_ReturnsFalse()
        {
            bool found = grid.QueryClosest(
                Vector2.zero,
                out _,
                out _
            );

            Assert.IsFalse(found);
        }

        [Test]
        public void QueryClosest_SingleItem_ReturnsItem()
        {
            grid.Add("A", new Vector2(5, 5));

            bool found = grid.QueryClosest(
                new Vector2(0, 0),
                out Vector2 point,
                out string item
            );

            Assert.IsTrue(found);
            Assert.AreEqual("A", item);
            Assert.AreEqual(new Vector2(5, 5), point);
        }

        [Test]
        public void QueryClosest_ReturnsNearestItem()
        {
            grid.Add("A", new Vector2(2, 2));
            grid.Add("B", new Vector2(10, 10));
            grid.Add("C", new Vector2(15, 15));

            bool found = grid.QueryClosest(
                new Vector2(3, 3),
                out Vector2 point,
                out string item
            );

            Assert.IsTrue(found);
            Assert.AreEqual("A", item);
            Assert.AreEqual(new Vector2(2, 2), point);
        }

        [Test]
        public void QueryClosest_ItemsInDifferentCells_ReturnsNearest()
        {
            grid.Add("A", new Vector2(1, 1));
            grid.Add("B", new Vector2(8, 8));
            grid.Add("C", new Vector2(12, 12));

            bool found = grid.QueryClosest(
                new Vector2(9, 9),
                out Vector2 point,
                out string item
            );

            Assert.IsTrue(found);
            Assert.AreEqual("B", item);
            Assert.AreEqual(new Vector2(8, 8), point);
        }

        [Test]
        public void QueryClosest_ExactPositionMatch_ReturnsSameItem()
        {
            grid.Add("A", new Vector2(5, 5));
            grid.Add("B", new Vector2(6, 6));

            bool found = grid.QueryClosest(
                new Vector2(5, 5),
                out Vector2 point,
                out string item
            );

            Assert.IsTrue(found);
            Assert.AreEqual("A", item);
            Assert.AreEqual(new Vector2(5, 5), point);
        }

        [Test]
        public void QueryClosest_MultipleItemsInSameCell_ReturnsNearest()
        {
            grid.Add("A", new Vector2(5.10f, 5.10f));
            grid.Add("B", new Vector2(5.80f, 5.80f));
            grid.Add("C", new Vector2(5.30f, 5.30f));

            bool found = grid.QueryClosest(
                new Vector2(5.11f, 5.11f),
                out Vector2 point,
                out string item
            );

            Assert.IsTrue(found);
            Assert.AreEqual("A", item);
            Assert.AreEqual(new Vector2(5.10f, 5.10f), point);
        }

        [Test]
        public void QueryClosest_PositionOutsideGrid_FindsNearestObject()
        {
            grid.Add("A", new Vector2(5, 5));

            bool found = grid.QueryClosest(
                new Vector2(-10, -10),
                out Vector2 point,
                out string item);

            Assert.IsTrue(found);
            Assert.AreEqual("A", item);
            Assert.AreEqual(new Vector2(5, 5), point);
        }
        
        [Test]
        public void QueryClosest_PositionFarOutsideGrid_FindsNearestObject()
        {
            grid.Add("A", new Vector2(5, 5));
            grid.Add("B", new Vector2(8, 8));

            bool found = grid.QueryClosest(
                new Vector2(1000, 1000),
                out _,
                out string item);

            Assert.IsTrue(found);
            Assert.AreEqual("B", item);
        }
    }
}