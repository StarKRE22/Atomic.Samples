using NUnit.Framework;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreeTests_Move
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
            tree.Add(3, new Vector2(8f, 8f));
        }

        [Test]
        public void Move_UnknownItem_ReturnsFalse()
        {
            Assert.IsFalse(
                tree.Move(999, new Vector2(2f, 2f))
            );
        }

        [Test]
        public void Move_InsideSameNode_UpdatesPosition()
        {
            Assert.IsTrue(
                tree.Move(2, new Vector2(5.2f, 5.1f))
            );

            bool found = tree.QueryClosest(
                new Vector2(5.2f, 5.1f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(2, item);
            Assert.AreEqual(new Vector2(5.2f, 5.1f), point);
        }

        [Test]
        public void Move_ToAnotherQuadrant_ItemIsStillFound()
        {
            Assert.IsTrue(
                tree.Move(1, new Vector2(9f, 1f))
            );

            bool found = tree.QueryClosest(
                new Vector2(9f, 1f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(1, item);
            Assert.AreEqual(new Vector2(9f, 1f), point);
        }

        [Test]
        public void Move_OutsideTree_ReturnsFalse()
        {
            Assert.IsFalse(
                tree.Move(2, new Vector2(100f, 100f))
            );
        }

        [Test]
        public void Move_OutsideTree_RollsBackToOriginalPosition()
        {
            Assert.IsFalse(
                tree.Move(2, new Vector2(100f, 100f))
            );

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
        public void Move_AfterSubdivision_WorksCorrectly()
        {
            tree.Add(4, new Vector2(2f, 8f));
            tree.Add(5, new Vector2(8f, 2f));
            
            bool moved = tree.Move(4, new Vector2(9f, 9f));
            Assert.IsTrue(moved);

            bool found = tree.QueryClosest(
                new Vector2(9f, 9f),
                out Vector2 point,
                out int item
            );

            Assert.IsTrue(found);
            Assert.AreEqual(4, item);
            Assert.AreEqual(new Vector2(9f, 9f), point);
        }

        [Test]
        public void Move_DoesNotDuplicateItem()
        {
            Assert.IsTrue(
                tree.Move(2, new Vector2(6f, 6f))
            );

            int[] results = new int[10];

            int count = tree.QueryRect(
                new Rect(0f, 0f, 10f, 10f),
                results
            );

            int occurrences = 0;

            for (int i = 0; i < count; i++)
            {
                if (results[i] == 2)
                    occurrences++;
            }

            Assert.AreEqual(1, occurrences);
        }
    }
}