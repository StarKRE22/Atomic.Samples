using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed partial class KdTree2DTests
    {
        [Test]
        public void FindValues_EmptyTree_ReturnsEmptyList()
        {
            var result = tree2D.QueryCircle(new Vector2(5, 5), 3f);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindValues_SingleNode_InsideRadius_ReturnsValue()
        {
            Vector2 point = new(5, 5);
            object value = new();

            tree2D.Add(value, point);

            var result = tree2D.QueryCircle(new Vector2(6, 6), 2f);

            Assert.AreEqual(1, result.Count);
            Assert.Contains(value, result);
        }

        [Test]
        public void FindValues_SingleNode_OutsideRadius_ReturnsEmptyList()
        {
            tree2D.Add(new object(), new Vector2(2, 2));

            var result = tree2D.QueryCircle(new Vector2(10, 10), 3f);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindValues_MultipleNodes_FindsAllWithinRadius()
        {
            object a = new();
            object b = new();
            object c = new();
            object d = new();

            tree2D.Add(a, new Vector2(1, 1));
            tree2D.Add(b, new Vector2(5, 5));
            tree2D.Add(c, new Vector2(7, 7));
            tree2D.Add(d, new Vector2(9, 9));

            var result = tree2D.QueryCircle(new Vector2(6, 6), 3f);

            Assert.AreEqual(2, result.Count);
            Assert.Contains(b, result);
            Assert.Contains(c, result);
        }

        [Test]
        public void FindValues_DuplicatePoints_ReturnsAllValues()
        {
            Vector2 point = new(5, 5);

            object a = new();
            object b = new();

            tree2D.Add(a, point);
            tree2D.Add(b, point);

            var result = tree2D.QueryCircle(point, 1f);

            Assert.AreEqual(2, result.Count);
            Assert.Contains(a, result);
            Assert.Contains(b, result);
        }

        [Test]
        public void FindValues_RadiusIsZero_ReturnsEmptyList()
        {
            tree2D.Add(new object(), new Vector2(4, 4));

            var result = tree2D.QueryCircle(new Vector2(4, 4), 0f);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindValues_NegativeRadius_ReturnsEmptyList()
        {
            tree2D.Add(new object(), new Vector2(4, 4));

            var result = tree2D.QueryCircle(new Vector2(4, 4), -1f);

            Assert.AreEqual(0, result.Count);
        }
    }
}