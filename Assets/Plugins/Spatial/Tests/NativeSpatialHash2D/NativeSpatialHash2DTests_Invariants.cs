using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace Modules.Spatial
{
    [TestFixture]
    public class NativeSpatialHash2DTests_Invariants
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
        public void Add_IncreasesCount()
        {
            _hash.Add(1, new float2(0, 0));

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Remove_DecreasesCount()
        {
            _hash.Add(1, new float2(0, 0));

            bool removed = _hash.Remove(1);

            Assert.IsTrue(removed);
            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Remove_LastItem_RemovesCell()
        {
            _hash.Add(1, new float2(0, 0));

            _hash.Remove(1);

            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Remove_OneOfMany_KeepsCellAlive()
        {
            _hash.Add(1, new float2(0.1f, 0.1f));
            _hash.Add(2, new float2(0.2f, 0.2f));

            _hash.Remove(1);

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Move_InsideSameCell_DoesNotCreateNewCell()
        {
            _hash.Add(1, new float2(0.1f, 0.1f));

            _hash.Move(1, new float2(0.9f, 0.9f));

            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Move_ToAnotherCell_KeepsCount()
        {
            _hash.Add(1, new float2(0.1f, 0.1f));

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
        public void Remove_UnknownItem_ReturnsFalse()
        {
            bool removed = _hash.Remove(123);

            Assert.IsFalse(removed);
            Assert.AreEqual(0, _hash.Count);
        }

        [Test]
        public void Move_UnknownItem_ReturnsFalse()
        {
            bool moved = _hash.Move(123, new float2(1, 1));

            Assert.IsFalse(moved);
            Assert.AreEqual(0, _hash.Count);
        }

        [Test]
        public void Add_Remove_AddAgain_Works()
        {
            _hash.Add(1, new float2(0, 0));

            _hash.Remove(1);

            bool added =
                _hash.Add(1, new float2(5, 5));

            Assert.IsTrue(added);
            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void ManyMoves_CountRemainsStable()
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
        public void ManyAddRemove_LeavesStructureEmpty()
        {
            for (int i = 0; i < 1000; i++)
            {
                _hash.Add(i, new float2(i, 0));
            }

            for (int i = 0; i < 1000; i++)
            {
                _hash.Remove(i);
            }

            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }
    }
}