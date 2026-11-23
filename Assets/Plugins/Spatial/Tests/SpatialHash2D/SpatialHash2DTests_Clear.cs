using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public class SpatialHash2DTests_Clear
    {
        private SpatialHash2D<string> _spatialHash;

        [SetUp]
        public void Setup()
        {
            _spatialHash = new SpatialHash2D<string>(1f);
        }

        [Test]
        public void Clear_AfterInsert_RemovesAllData()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Add("B", new Vector2(2, 2));

            // Act
            _spatialHash.Clear();

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(Vector2.zero, 10f, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Clear_AllowsReinsert()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Clear();

            // Act
            _spatialHash.Add("B", new Vector2(2, 2));

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(new Vector2(2, 2), 0.1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("B", buffer[0]);
        }

        [Test]
        public void Clear_MultipleTimes_NoErrors()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));

            // Act
            _spatialHash.Clear();
            _spatialHash.Clear();
            _spatialHash.Clear();

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(Vector2.zero, 10f, buffer);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Clear_RemovesLookupReferences()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Clear();

            // Act
            bool removed = _spatialHash.Remove("A");

            // Assert
            Assert.IsFalse(removed, "После Clear объект не должен существовать в lookup");
        }

        [Test]
        public void Clear_AfterMoveAndRemove_LeavesCleanState()
        {
            // Arrange
            _spatialHash.Add("A", new Vector2(1, 1));
            _spatialHash.Move("A", new Vector2(2, 2));
            _spatialHash.Remove("A");

            // Act
            _spatialHash.Clear();

            // Assert
            var buffer = new string[10];
            int count = _spatialHash.QueryCircle(Vector2.zero, 10f, buffer);

            Assert.AreEqual(0, count);
        }
    }
}