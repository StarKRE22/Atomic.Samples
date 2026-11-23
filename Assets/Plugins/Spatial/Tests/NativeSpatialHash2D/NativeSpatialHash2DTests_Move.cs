using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace Modules.Spatial
{
    [TestFixture]
    public class NativeSpatialHash2DTests_Move
    {
        private NativeSpatialHash2D<int> _hash;

        [SetUp]
        public void Setup()
        {
            _hash = new NativeSpatialHash2D<int>(
                Allocator.Temp,
                cellSize: 1f
            );
        }

        [TearDown]
        public void TearDown()
        {
            _hash.Dispose();
        }

        [Test]
        public void Move_ExistingItem_ReturnsTrue()
        {
            _hash.Add(1, new float2(1, 1));

            bool moved =
                _hash.Move(1, new float2(2, 2));

            Assert.IsTrue(moved);
            Assert.AreEqual(1, _hash.Count);
        }

        [Test]
        public void Move_MissingItem_ReturnsFalse()
        {
            bool moved =
                _hash.Move(1, new float2(1, 1));

            Assert.IsFalse(moved);
            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Move_InsideSameCell_DoesNotCreateCell()
        {
            _hash.Add(1, new float2(1.1f, 1.1f));

            int cellsBefore = _hash.CellCount;

            _hash.Move(1, new float2(1.8f, 1.8f));

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(cellsBefore, _hash.CellCount);
        }

        [Test]
        public void Move_ToDifferentCell_KeepsCount()
        {
            _hash.Add(1, new float2(0, 0));

            _hash.Move(1, new float2(5, 5));

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Move_LastItemFromCell_RemovesOldCell()
        {
            _hash.Add(1, new float2(0, 0));

            _hash.Move(1, new float2(10, 10));

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Move_OneOfMany_PreservesOldCell()
        {
            _hash.Add(1, new float2(0.1f, 0.1f));
            _hash.Add(2, new float2(0.2f, 0.2f));

            _hash.Move(1, new float2(10, 10));

            Assert.AreEqual(2, _hash.Count);
            Assert.AreEqual(2, _hash.CellCount);
        }

        [Test]
        public void Move_MultipleItems_OnlyMovesTarget()
        {
            _hash.Add(1, new float2(0, 0));
            _hash.Add(2, new float2(0, 0));

            _hash.Move(1, new float2(10, 10));

            Assert.AreEqual(2, _hash.Count);
            Assert.AreEqual(2, _hash.CellCount);
        }

        [Test]
        public void Move_ManyTimes_CountRemainsStable()
        {
            _hash.Add(1, new float2(0, 0));

            for (int i = 0; i < 1000; i++)
            {
                _hash.Move(1, new float2(i, i));
            }

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Move_BackAndForth_DoesNotLeakCells()
        {
            _hash.Add(1, new float2(0, 0));

            for (int i = 0; i < 1000; i++)
            {
                _hash.Move(1, new float2(10, 10));
                _hash.Move(1, new float2(0, 0));
            }

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Move_ManyObjects_CountRemainsCorrect()
        {
            const int count = 100;

            for (int i = 0; i < count; i++)
            {
                _hash.Add(i, new float2(i, 0));
            }

            for (int i = 0; i < count; i++)
            {
                _hash.Move(i, new float2(i + 1000, 0));
            }

            Assert.AreEqual(count, _hash.Count);
        }

        [Test]
        public void Move_ToNegativeCoordinates_Works()
        {
            _hash.Add(1, new float2(10, 10));

            bool moved =
                _hash.Move(1, new float2(-5, -5));

            Assert.IsTrue(moved);
            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }
        
        [Test]
        public void Move_CausesCellResize_DoesNotLoseItem()
        {
            _hash.Add(0, new float2(0, 0));

            for (int i = 1; i < 100; i++)
                _hash.Add(i, new float2(10, 10));

            bool moved = _hash.Move(0, new float2(10, 10));

            Assert.IsTrue(moved);
            Assert.AreEqual(100, _hash.Count);
        }
    }
}