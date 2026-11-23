using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public class SpatialHash2DTests_Move
    {
        private SpatialHash2D<string> _spatialHash;

        [SetUp]
        public void Setup()
        {
            _spatialHash = new SpatialHash2D<string>(1f);
        }

        [Test]
        public void Move_ObjectExists_ChangesPosition()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));

            // Act
            _spatialHash.Move("A", new Vector2(2, 2));

            // Assert
            var buffer = new string[10];

            int oldCount = _spatialHash.QueryCircle(new Vector2(1, 1), 0.1f, buffer);
            int newCount = _spatialHash.QueryCircle(new Vector2(2, 2), 0.1f, buffer);

            Assert.AreEqual(0, oldCount);
            Assert.AreEqual(1, newCount);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Move_SameCell_OnlyUpdatesPosition()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1.1f, 1.1f)); // cell (1,1)

            // Act
            _spatialHash.Move("A", new Vector2(1.2f, 1.2f)); // та же ячейка

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(1.2f, 1.2f), 0.2f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Move_ToDifferentCell_RemovesFromOldAndAddsToNew()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(0, 0));

            // Act
            _spatialHash.Move("A", new Vector2(5, 5));

            // Assert
            var buffer = new string[10];

            int oldCount = _spatialHash.QueryCircle(new Vector2(0, 0), 1f, buffer);
            int newCount = _spatialHash.QueryCircle(new Vector2(5, 5), 1f, buffer);

            Assert.AreEqual(0, oldCount);
            Assert.AreEqual(1, newCount);
        }

        [Test]
        public void Move_ObjectDoesNotExist_DoesNothing()
        {
            // Act
            _spatialHash.Move("A", new Vector2(1, 1));

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(1, 1), 1f, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Move_MultipleObjects_OnlyMovesTarget()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(0, 0));
            _spatialHash.Add("B", new Vector2(0, 0));

            // Act
            _spatialHash.Move("A", new Vector2(10, 10));

            // Assert
            var bufferOld = new string[10];
            var bufferNew = new string[10];

            int oldCount = _spatialHash.QueryCircle(new Vector2(0, 0), 1f, bufferOld);
            int newCount = _spatialHash.QueryCircle(new Vector2(10, 10), 1f, bufferNew);

            Assert.AreEqual(1, oldCount);
            Assert.AreEqual("B", bufferOld[0]);

            Assert.AreEqual(1, newCount);
            Assert.AreEqual("A", bufferNew[0]);
        }

        [Test]
        public void Move_ManyTimes_NoGhostEntries()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(0, 0));

            // Act
            for (int i = 0; i < 10; i++)
            {
                _spatialHash.Move("A", new Vector2(i, i));
            }

            // Assert
            var buffer = new string[20];
            int count = _spatialHash.QueryCircle(new Vector2(9, 9), 0.1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Move_BufferOverflowHandled()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(0, 0));
            _spatialHash.Add("B", new Vector2(0, 0));
            _spatialHash.Add("C", new Vector2(0, 0));

            var buffer = new string[2];

            // Act
            _spatialHash.Move("A", new Vector2(1, 1));
            int count = _spatialHash.QueryCircle(new Vector2(0, 0), 1f, buffer);

            // Assert
            Assert.AreEqual(2, count);
        }
    }
}