using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    [TestFixture]
    public sealed partial class KdTree2DTests
    {
        [Test]
        public void FindMin_EmptyTree_ReturnsNull()
        {
            var minNodeX = tree2D.FindMin(null, KdTree2D<object>.Axis.X, 0);
            var minNodeY = tree2D.FindMin(null, KdTree2D<object>.Axis.Y, 0);

            Assert.IsNull(minNodeX);
            Assert.IsNull(minNodeY);
        }

        [Test]
        public void FindMin_SingleNode_ReturnsSameNode()
        {
            Vector2 point = new(5, 5);

            tree2D.Add(new object(), point);

            var minNodeX = tree2D.FindMin(tree2D.root, KdTree2D<object>.Axis.X, 0);
            var minNodeY = tree2D.FindMin(tree2D.root, KdTree2D<object>.Axis.Y, 0);

            Assert.IsNotNull(minNodeX);
            Assert.AreEqual(point, minNodeX.point);

            Assert.IsNotNull(minNodeY);
            Assert.AreEqual(point, minNodeY.point);
        }

        [Test]
        public void FindMin_MultipleNodes_FindsMinimumOnXAxis()
        {
            tree2D.Add(new object(), new Vector2(7, 2));
            tree2D.Add(new object(), new Vector2(3, 6));
            tree2D.Add(new object(), new Vector2(2, 8));
            tree2D.Add(new object(), new Vector2(5, 4));
            tree2D.Add(new object(), new Vector2(9, 1));

            var minNodeX = tree2D.FindMin(tree2D.root, KdTree2D<object>.Axis.X, 0);

            Assert.IsNotNull(minNodeX);
            Assert.AreEqual(new Vector2(2, 8), minNodeX.point);
        }

        [Test]
        public void FindMin_MultipleNodes_FindsMinimumOnYAxis()
        {
            tree2D.Add(new object(), new Vector2(7, 2));
            tree2D.Add(new object(), new Vector2(3, 6));
            tree2D.Add(new object(), new Vector2(2, 8));
            tree2D.Add(new object(), new Vector2(5, 4));
            tree2D.Add(new object(), new Vector2(9, 1));

            var minNodeY = tree2D.FindMin(tree2D.root, KdTree2D<object>.Axis.Y, 0);

            Assert.IsNotNull(minNodeY);
            Assert.AreEqual(new Vector2(9, 1), minNodeY.point);
        }

        [Test]
        public void FindMin_UnbalancedTree_FindsCorrectMinimum()
        {
            tree2D.Add(new object(), new Vector2(5, 5));
            tree2D.Add(new object(), new Vector2(3, 4));
            tree2D.Add(new object(), new Vector2(2, 3));
            tree2D.Add(new object(), new Vector2(1, 2));
            tree2D.Add(new object(), new Vector2(6, 1));

            var minNodeX = tree2D.FindMin(tree2D.root, KdTree2D<object>.Axis.X, 0);
            var minNodeY = tree2D.FindMin(tree2D.root, KdTree2D<object>.Axis.Y, 0);

            Assert.AreEqual(new Vector2(1, 2), minNodeX.point);
            Assert.AreEqual(new Vector2(6, 1), minNodeY.point);
        }
    }
}