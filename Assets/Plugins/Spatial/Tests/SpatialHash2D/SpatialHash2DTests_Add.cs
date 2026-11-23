using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public class SpatialHash2DTests_Add
    {
        private SpatialHash2D<string> _hash;

        [SetUp]
        public void Setup()
        {
            _hash = new SpatialHash2D<string>(1f);
        }

        [Test]
        public void Add_SingleElement_CanBeQueried()
        {
            // Arrange
            var pos = new Vector2(1, 1);

            // Act
            _hash.Add("A", pos);

            // Assert
            var buffer = new string[10];
            int count = _hash.QueryCircle(pos, 0.1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Add_MultipleElements_SameCell_AllStored()
        {
            // Arrange
            var pos = new Vector2(1.2f, 1.3f);

            // Act
            _hash.Add("A", pos);
            _hash.Add("B", pos);
            _hash.Add("C", pos);

            // Assert
            var buffer = new string[10];
            int count = _hash.QueryCircle(pos, 0.5f, buffer);

            Assert.AreEqual(3, count);
            CollectionAssert.Contains(buffer, "A");
            CollectionAssert.Contains(buffer, "B");
            CollectionAssert.Contains(buffer, "C");
        }

        [Test]
        public void Add_DifferentCells_StoredSeparately()
        {
            // Arrange
            _hash.Add("A", new Vector2(0, 0));
            _hash.Add("B", new Vector2(10, 10));

            // Act
            var buffer = new string[10];
            int count = _hash.QueryCircle(new Vector2(0, 0), 1f, buffer);

            // Assert
            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Add_NegativeCoordinates_WorksCorrectly()
        {
            // Arrange
            var pos = new Vector2(-1.2f, -3.4f);

            // Act
            _hash.Add("A", pos);

            // Assert
            var buffer = new string[10];
            int count = _hash.QueryCircle(pos, 0.5f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

      
        [Test]
        public void Add_SameObject_ReturnsFalse()
        {
            var pos1 = new Vector2(1, 1);
            var pos2 = new Vector2(2, 2);

            bool first = _hash.Add("A", pos1);
            bool second = _hash.Add("A", pos2);

            Assert.IsTrue(first);
            Assert.IsFalse(second);

            var buffer = new string[10];

            int count = _hash.QueryCircle(pos1, 0.1f, buffer);

            Assert.AreEqual(1, count);
            Assert.AreEqual("A", buffer[0]);
        }

        [Test]
        public void Add_BufferOverflow_IsClamped()
        {
            // Arrange
            var pos = new Vector2(1, 1);

            _hash.Add("A", pos);
            _hash.Add("B", pos);
            _hash.Add("C", pos);

            var buffer = new string[2];

            // Act
            int count = _hash.QueryCircle(pos, 1f, buffer);

            // Assert
            Assert.AreEqual(2, count);
        }
    }
}