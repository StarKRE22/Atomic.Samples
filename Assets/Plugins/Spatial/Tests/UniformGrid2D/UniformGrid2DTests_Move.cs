using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class UniformGrid2DTests_Move
    {
        private UniformGrid2D<int> grid;

        [SetUp]
        public void Setup()
        {
            grid = new UniformGrid2D<int>(10, 10, 1f);
        }

        [Test]
        public void Move_ShouldReturnFalse_WhenObjectNotExists()
        {
            bool result = grid.Move(1, new Vector2(1, 1));
            Assert.IsFalse(result);
        }

        [Test]
        public void Move_ShouldReturnFalse_WhenOutOfBounds()
        {
            grid.Add(1, new Vector2(1, 1));

            bool result = grid.Move(1, new Vector2(-1, 0));

            Assert.IsFalse(result);
            Assert.AreEqual(1, grid.GetCount(1, 1));
        }

        [Test]
        public void Move_ShouldStayInSameCell_WhenPositionInsideCell()
        {
            grid.Add(1, new Vector2(1.2f, 1.2f));

            bool result = grid.Move(1, new Vector2(1.8f, 1.8f));

            Assert.IsTrue(result);
            Assert.AreEqual(1, grid.GetCount(1, 1));
            Assert.AreEqual(1, grid[1, 1, 0]);
        }

        [Test]
        public void Move_ShouldMoveToAnotherCell()
        {
            grid.Add(1, new Vector2(1, 1));

            bool result = grid.Move(1, new Vector2(2, 2));

            Assert.IsTrue(result);

            Assert.AreEqual(0, grid.GetCount(1, 1));
            Assert.AreEqual(1, grid.GetCount(2, 2));
            Assert.AreEqual(1, grid[2, 2, 0]);
        }

        [Test]
        public void Move_ShouldUpdateCellCountsCorrectly()
        {
            grid.Add(1, new Vector2(1, 1));
            grid.Add(2, new Vector2(2, 2));

            grid.Move(1, new Vector2(2, 2));

            Assert.AreEqual(0, grid.GetCount(1, 1));
            Assert.AreEqual(2, grid.GetCount(2, 2));
        }

        [Test]
        public void Move_ShouldHandleSwapBackCorrectly()
        {
            grid.Add(1, new Vector2(1, 1));
            grid.Add(2, new Vector2(1, 1));

            grid.Move(1, new Vector2(2, 2));

            Assert.AreEqual(1, grid.GetCount(1, 1));
            Assert.AreEqual(2, grid[1, 1, 0]);

            Assert.AreEqual(1, grid.GetCount(2, 2));
            Assert.AreEqual(1, grid.GetValue(2, 2, 0));
        }

        [Test]
        public void Move_ShouldAllowFurtherOperationsAfterMove()
        {
            grid.Add(1, new Vector2(1, 1));

            grid.Move(1, new Vector2(2, 2));
            bool removed = grid.Remove(1);

            Assert.IsTrue(removed);
            Assert.AreEqual(0, grid.GetCount(2, 2));
        }

        [Test]
        public void Move_ShouldHandleManyObjects()
        {
            for (int i = 0; i < 1000; i++)
                grid.Add(i, new Vector2(i % 10, i % 10));

            for (int i = 0; i < 1000; i++)
            {
                bool result = grid.Move(i, new Vector2((i + 1) % 10, (i + 1) % 10));
                Assert.IsTrue(result);
            }
        }
    }
}