using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public sealed partial class KdTree2DTests
    {
        [Test]
        public void Add_SinglePoint_IncreasesCount()
        {
            // Arrange
            Vector2 point = new(1, 2);

            // Act
            bool success = tree2D.Add(new object(), point);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1, tree2D.Count);
        }

        [Test]
        public void Add_MultiplePoints_IncreasesCountCorrectly()
        {
            // Arrange
            Vector2[] points =
            {
                new(1, 2),
                new(3, 4),
                new(5, 6)
            };

            // Act
            foreach (Vector2 point in points)
                tree2D.Add(new object(), point);

            // Assert
            Assert.AreEqual(3, tree2D.Count);
        }

        [Test]
        public void Add_DifferentValuesAtSamePoint_AddsMultipleValues()
        {
            // Arrange
            Vector2 point = new(1, 2);

            // Act
            tree2D.Add("A", point);
            tree2D.Add("B", point);

            // Assert
            Assert.AreEqual(2, tree2D.Count);

            bool found = tree2D.QueryPoint(point, out var values);

            Assert.IsTrue(found);
            Assert.AreEqual(2, values.Count);

            CollectionAssert.Contains(values, "A");
            CollectionAssert.Contains(values, "B");
        }

        [Test]
        public void Add_PointsInDifferentQuadrants_ShouldMaintainCorrectCount()
        {
            // Arrange
            Vector2[] points =
            {
                new(-1, -2),
                new(3, 4),
                new(5, -6),
                new(-7, 8)
            };

            // Act
            foreach (Vector2 point in points)
                tree2D.Add(new object(), point);

            // Assert
            Assert.AreEqual(4, tree2D.Count);
        }

        [Test]
        public void Add_CheckTreeStructureWithTwoPoints()
        {
            // Arrange
            Vector2 root = new(5, 5);
            Vector2 leftChild = new(2, 2);

            // Act
            tree2D.Add("root", root);
            tree2D.Add("child", leftChild);

            // Assert
            Assert.AreEqual(2, tree2D.Count);

            Assert.IsTrue(tree2D.QueryPoint(root, out var rootValues));
            Assert.IsTrue(tree2D.QueryPoint(leftChild, out var childValues));

            Assert.AreEqual(1, rootValues.Count);
            Assert.AreEqual(1, childValues.Count);

            CollectionAssert.Contains(rootValues, "root");
            CollectionAssert.Contains(childValues, "child");
        }

        [Test]
        public void Add_SameValueTwice_ReturnsFalse()
        {
            // Arrange
            object value = new();
            Vector2 point1 = new(1, 1);
            Vector2 point2 = new(2, 2);

            // Act
            bool first = tree2D.Add(value, point1);
            bool second = tree2D.Add(value, point2);

            // Assert
            Assert.IsTrue(first);
            Assert.IsFalse(second);
            Assert.AreEqual(1, tree2D.Count);
        }
        
        [Test]
        public void Add_1000RandomValues_CountMatches()
        {
            const int count = 1000;

            for (int i = 0; i < count; i++)
            {
                tree2D.Add(
                    i,
                    new Vector2(
                        Random.Range(0f, 1000f),
                        Random.Range(0f, 1000f)
                    )
                );
            }

            Assert.AreEqual(count, tree2D.Count);
        }
        
        [Test]
        public void Add_1000RandomValues_AllValuesCanBeRemoved()
        {
            const int count = 1000;

            for (int i = 0; i < count; i++)
            {
                tree2D.Add(
                    i,
                    new Vector2(
                        Random.Range(0f, 1000f),
                        Random.Range(0f, 1000f)
                    )
                );
            }

            Assert.AreEqual(count, tree2D.Count);

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(tree2D.Remove(i));
            }

            Assert.AreEqual(0, tree2D.Count);
        }
    }
}