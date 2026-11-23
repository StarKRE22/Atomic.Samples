using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class UniformGrid2DTests_Remove
    {
        private UniformGrid2D<int> grid;

        [SetUp]
        public void Setup()
        {
            grid = new UniformGrid2D<int>(10, 10, 1f);
        }

        [Test]
        public void Remove_ShouldReturnTrue_WhenExists()
        {
            grid.Add(1, new Vector2(1, 1));

            bool result = grid.Remove(1);

            Assert.IsTrue(result);
        }

        [Test]
        public void Remove_ShouldReturnFalse_WhenNotExists()
        {
            bool result = grid.Remove(1);

            Assert.IsFalse(result);
        }

        [Test]
        public void Remove_ShouldRemoveObjectFromCell()
        {
            grid.Add(1, new Vector2(2, 3));

            grid.Remove(1);

            int count = grid.GetCount(2, 3);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Remove_ShouldNotAffectOtherObjects()
        {
            grid.Add(1, new Vector2(2, 3));
            grid.Add(2, new Vector2(2, 3));

            grid.Remove(1);

            int count = grid.GetCount(2, 3);

            Assert.AreEqual(1, count);
            Assert.AreEqual(2, grid.GetValue(2, 3, 0));
        }

        [Test]
        public void Remove_ShouldHandleSwapBackCorrectly()
        {
            grid.Add(1, new Vector2(2, 3));
            grid.Add(2, new Vector2(2, 3));

            grid.Remove(1);

            int count = grid.GetCount(2, 3);

            Assert.AreEqual(1, count);
            Assert.AreEqual(2, grid.GetValue(2, 3, 0));

            bool removedSecond = grid.Remove(2);

            Assert.IsTrue(removedSecond);
            Assert.AreEqual(0, grid.GetCount(2, 3));
        }

        [Test]
        public void Remove_ShouldAllowReinsertAfterRemoval()
        {
            grid.Add(1, new Vector2(1, 1));
            grid.Remove(1);

            bool result = grid.Add(1, new Vector2(1, 1));

            Assert.IsTrue(result);
            Assert.AreEqual(1, grid.GetCount(1, 1));
        }

        [Test]
        public void Remove_ShouldHandleManyObjects()
        {
            for (int i = 0; i < 1000; i++)
                grid.Add(i, new Vector2(i % 10, i % 10));

            for (int i = 0; i < 1000; i++)
            {
                bool result = grid.Remove(i);
                Assert.IsTrue(result);
            }

            for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                Assert.AreEqual(0, grid.GetCount(x, y));
        }
    }
}