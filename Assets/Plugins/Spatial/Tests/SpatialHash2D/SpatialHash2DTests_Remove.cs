using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public class SpatialHash2DTests_Remove
    {
        private SpatialHash2D<string> _spatialHash;

        [SetUp]
        public void Setup()
        {
            _spatialHash = new SpatialHash2D<string>(1f);
        }

        [Test]
        public void Remove_ExistingObject_RemovesSuccessfully()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));

            // Act
            bool removed = _spatialHash.Remove("A");

            // Assert
            Assert.IsTrue(removed);

            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(1, 1), 0.1f, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Remove_ObjectDoesNotExist_ReturnsFalse()
        {
            // Act
            bool removed = _spatialHash.Remove("A");

            // Assert
            Assert.IsFalse(removed);
        }

        [Test]
        public void Remove_FromSingleElementCell_RemovesCell()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));

            // Act
            _spatialHash.Remove("A");

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(1, 1), 1f, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Remove_MultipleElements_OnlyRemovesTarget()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Add("B", new Vector2(1, 1));
            _spatialHash.Add("C", new Vector2(1, 1));

            // Act
            _spatialHash.Remove("B");

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(1, 1), 1f, buffer);

            Assert.AreEqual(2, count);
            CollectionAssert.Contains(buffer, "A");
            CollectionAssert.Contains(buffer, "C");
        }

        [Test]
        public void Remove_SwapLogic_KeepsStructureValid()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Add("B", new Vector2(1, 1));
            _spatialHash.Add("C", new Vector2(1, 1));

            // Act
            _spatialHash.Remove("A"); // триггер swap

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(1, 1), 1f, buffer);

            Assert.AreEqual(2, count);
            CollectionAssert.DoesNotContain(buffer, "A");
        }

        [Test]
        public void Remove_AllElements_LeavesEmptyStructure()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Add("B", new Vector2(2, 2));

            // Act
            _spatialHash.Remove("A");
            _spatialHash.Remove("B");

            // Assert
            var buffer = new string[10];

            int count1 = _spatialHash.QueryCircle(new Vector2(1, 1), 1f, buffer);
            int count2 = _spatialHash.QueryCircle(new Vector2(2, 2), 1f, buffer);

            Assert.AreEqual(0, count1);
            Assert.AreEqual(0, count2);
        }

        [Test]
        public void Remove_ThenInsertAgain_WorksCorrectly()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Remove("A");

            // Act
            _spatialHash.Add("A", new Vector2(2, 2));

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(2, 2), 0.1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Remove_ManyOperations_NoGhostEntries()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(0, 0));

            // Act
            for (int i = 0; i < 10; i++)
            {
                _spatialHash.Move("A", new Vector2(i, i));
                _spatialHash.Remove("A");
                _spatialHash.Add("A", new Vector2(i, i));
            }

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(9, 9), 0.1f, buffer);

            Assert.AreEqual(1, count);
        }
    }
}