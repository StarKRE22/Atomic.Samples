using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed partial class KdTree2DTests
    {
        [Test]
        public void Move_ValueExists_MovesToNewPoint()
        {
            Vector2 from = new(1, 1);
            Vector2 to = new(5, 5);

            tree2D.Add("A", from);

            bool success = tree2D.Move("A", to);

            Assert.IsTrue(success);

            Assert.IsFalse(tree2D.Contains(from));

            Assert.IsTrue(tree2D.QueryPoint(to, out var values));
            CollectionAssert.Contains(values, "A");

            Assert.AreEqual(1, tree2D.Count);
        }

        [Test]
        public void Move_ValueDoesNotExist_ReturnsFalse()
        {
            bool success = tree2D.Move("A", new Vector2(5, 5));

            Assert.IsFalse(success);
            Assert.AreEqual(0, tree2D.Count);
        }

        [Test]
        public void Move_ToExistingPoint_AddsValueToExistingNode()
        {
            Vector2 from = new(1, 1);
            Vector2 to = new(5, 5);

            tree2D.Add("A", from);
            tree2D.Add("B", to);

            bool success = tree2D.Move("A", to);

            Assert.IsTrue(success);

            Assert.IsFalse(tree2D.Contains(from));

            Assert.IsTrue(tree2D.QueryPoint(to, out var values));

            Assert.AreEqual(2, values.Count);
            CollectionAssert.Contains(values, "A");
            CollectionAssert.Contains(values, "B");

            Assert.AreEqual(2, tree2D.Count);
        }

        [Test]
        public void Move_OneOfSeveralValues_RemovesOnlyMovedValue()
        {
            Vector2 from = new(1, 1);
            Vector2 to = new(5, 5);

            tree2D.Add("A", from);
            tree2D.Add("B", from);

            bool success = tree2D.Move("A", to);

            Assert.IsTrue(success);

            Assert.IsTrue(tree2D.QueryPoint(from, out var sourceValues));
            Assert.AreEqual(1, sourceValues.Count);
            CollectionAssert.Contains(sourceValues, "B");

            Assert.IsTrue(tree2D.QueryPoint(to, out var targetValues));
            CollectionAssert.Contains(targetValues, "A");

            Assert.AreEqual(2, tree2D.Count);
        }

        [Test]
        public void Move_SamePoint_ReturnsTrueAndDoesNothing()
        {
            Vector2 point = new(1, 1);

            tree2D.Add("A", point);

            bool success = tree2D.Move("A", point);

            Assert.IsTrue(success);

            Assert.IsTrue(tree2D.QueryPoint(point, out var values));
            Assert.AreEqual(1, values.Count);

            CollectionAssert.Contains(values, "A");
            Assert.AreEqual(1, tree2D.Count);
        }

        [Test]
        public void Move_LastValueFromNode_RemovesSourceNode()
        {
            Vector2 from = new(1, 1);
            Vector2 to = new(10, 10);

            tree2D.Add("A", from);
            
            bool success = tree2D.Move("A", to);

            Assert.IsTrue(success);

            Assert.IsFalse(tree2D.Contains(from));
            Assert.IsTrue(tree2D.Contains(to));

            Assert.AreEqual(1, tree2D.Count);
        }
    }
}