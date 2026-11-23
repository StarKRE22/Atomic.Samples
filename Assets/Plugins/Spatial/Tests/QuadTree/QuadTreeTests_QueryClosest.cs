using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreeTests_QueryClosest
    {
        private QuadTree<int> tree;

        [SetUp]
        public void Setup()
        {
            tree = new QuadTree<int>(
                new Vector2(0f, 0f),
                new Vector2(10f, 10f),
                capacity: 2,
                maxDepth: 4
            );

            tree.Add(1, new Vector2(1f, 1f));
            tree.Add(2, new Vector2(5f, 5f));
            tree.Add(3, new Vector2(8f, 5f));
            tree.Add(4, new Vector2(9f, 9f));
        }

        [Test]
        public void QueryClosest_ZeroDistance()
        {
            tree.Add(4, new Vector2(9f, 9f));
            tree.Add(5, new Vector2(8f, 2f));

            bool found = tree.QueryClosest(new Vector2(9f, 9f), out Vector2 point, out int item);

            Assert.IsTrue(found);
            Assert.AreEqual(4, item);
            Assert.AreEqual(new Vector2(9f, 9f), point);
        }

        
        [Test]
        public void QueryClosest_EmptyTree_ReturnsFalse()
        {
            tree.Clear();

            bool found = tree.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int item
            );

            Assert.IsFalse(found);
            Assert.AreEqual(Vector2.zero, point);
            Assert.AreEqual(0, item);
        }

        [Test]
        public void QueryClosest_ExactMatch_ReturnsItem()
        {
            bool found = tree.QueryClosest(
                new Vector2(5f, 5f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(2, item);
            Assert.AreEqual(new Vector2(5f, 5f), point);
        }

        [Test]
        public void QueryClosest_ReturnsNearestPoint()
        {
            bool found = tree.QueryClosest(
                new Vector2(7f, 5f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(3, item);
            Assert.AreEqual(new Vector2(8f, 5f), point);
        }

        [Test]
        public void QueryClosest_PointOutsideTree_ReturnsNearest()
        {
            bool found = tree.QueryClosest(
                new Vector2(100f, 100f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(4, item);
            Assert.AreEqual(new Vector2(9f, 9f), point);
        }

        [Test]
        public void QueryClosest_SingleItem_ReturnsThatItem()
        {
            tree.Clear();

            tree.Add(
                42,
                new Vector2(3f, 7f)
            );

            bool found = tree.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(42, item);
            Assert.AreEqual(new Vector2(3f, 7f), point);
        }

        [Test]
        public void QueryClosest_AfterSubdivision_ReturnsCorrectItem()
        {
            tree.Add(5, new Vector2(2f, 8f));
            tree.Add(6, new Vector2(8f, 2f));

            bool found = tree.QueryClosest(
                new Vector2(7.9f, 2.1f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(6, item);
            Assert.AreEqual(new Vector2(8f, 2f), point);
        }

        [Test]
        public void QueryClosest_AfterMove_ReturnsUpdatedPosition()
        {
            tree.Move(
                2,
                new Vector2(0.5f, 0.5f)
            );

            bool found = tree.QueryClosest(
                Vector2.zero,
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(2, item);
            Assert.AreEqual(new Vector2(0.5f, 0.5f), point);
        }
    }
}