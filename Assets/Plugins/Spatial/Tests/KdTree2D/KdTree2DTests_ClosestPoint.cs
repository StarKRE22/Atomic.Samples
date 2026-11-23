using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public sealed partial class KdTree2DTests
    {
        [Test]
        public void ClosestPoint_EmptyTree_ReturnsFalse()
        {
            bool result = tree2D.QueryClosest(
                new Vector2(5, 5),
                out Vector2 closest,
                out object value);

            Assert.IsFalse(result);
        }

        [Test]
        public void ClosestPoint_SingleNode_ReturnsSamePointAndValue()
        {
            Vector2 point = new(5, 5);
            object obj = new();

            tree2D.Add(obj, point);

            bool result = tree2D.QueryClosest(
                new Vector2(7, 7),
                out Vector2 closest,
                out object value);

            Assert.IsTrue(result);
            Assert.AreEqual(point, closest);
            Assert.AreSame(obj, value);
        }

        [Test]
        public void ClosestPoint_MultipleNodes_FindsNearest()
        {
            object a = new();
            object b = new();
            object c = new();
            object d = new();
            object e = new();

            tree2D.Add(a, new Vector2(2, 3));
            tree2D.Add(b, new Vector2(5, 4));
            tree2D.Add(c, new Vector2(9, 6));
            tree2D.Add(d, new Vector2(4, 7));
            tree2D.Add(e, new Vector2(8, 1));

            Vector2 target = new(6, 5);

            bool result = tree2D.QueryClosest(
                target,
                out Vector2 closest,
                out object value);

            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(5, 4), closest);
            Assert.AreSame(b, value);
        }

        [Test]
        public void ClosestPoint_TargetExactlyOnNode_ReturnsSamePointAndValue()
        {
            Vector2 exactPoint = new(3, 3);
            object obj = new();

            tree2D.Add(new object(), new Vector2(5, 5));
            tree2D.Add(new object(), new Vector2(7, 7));
            tree2D.Add(obj, exactPoint);

            bool result = tree2D.QueryClosest(
                exactPoint,
                out Vector2 closest,
                out object value);

            Assert.IsTrue(result);
            Assert.AreEqual(exactPoint, closest);
            Assert.AreSame(obj, value);
        }

        [Test]
        public void ClosestPoint_CheckDifferentQuadrants()
        {
            object a = new();
            object b = new();
            object c = new();
            object d = new();

            tree2D.Add(a, new Vector2(1, 1));
            tree2D.Add(b, new Vector2(-2, 2));
            tree2D.Add(c, new Vector2(3, -3));
            tree2D.Add(d, new Vector2(-4, -4));

            tree2D.QueryClosest(
                new Vector2(0, 0),
                out Vector2 closest1,
                out object value1);

            Assert.AreEqual(new Vector2(1, 1), closest1);
            Assert.AreSame(a, value1);

            tree2D.QueryClosest(
                new Vector2(-3, 3),
                out Vector2 closest2,
                out object value2);

            Assert.AreEqual(new Vector2(-2, 2), closest2);
            Assert.AreSame(b, value2);

            tree2D.QueryClosest(
                new Vector2(4, -4),
                out Vector2 closest3,
                out object value3);

            Assert.AreEqual(new Vector2(3, -3), closest3);
            Assert.AreSame(c, value3);

            tree2D.QueryClosest(
                new Vector2(-5, -5),
                out Vector2 closest4,
                out object value4);

            Assert.AreEqual(new Vector2(-4, -4), closest4);
            Assert.AreSame(d, value4);
        }
    }
}