using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace Modules.Spatial
{
    [TestFixture]
    public class NativeSpatialHash2DTests_Clear
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
        public void Clear_AfterInsert_RemovesAllData()
        {
            _hash.Add(1, new float2(1, 1));
            _hash.Add(2, new float2(2, 2));

            _hash.Clear();

            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Clear_AllowsReinsert()
        {
            _hash.Add(1, new float2(1, 1));

            _hash.Clear();

            bool added =
                _hash.Add(2, new float2(2, 2));

            Assert.IsTrue(added);
            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Clear_MultipleTimes_NoErrors()
        {
            _hash.Add(1, new float2(1, 1));

            _hash.Clear();
            _hash.Clear();
            _hash.Clear();

            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Clear_RemovesLookupReferences()
        {
            _hash.Add(1, new float2(1, 1));

            _hash.Clear();

            bool removed =
                _hash.Remove(1);

            Assert.IsFalse(removed);
            Assert.AreEqual(0, _hash.Count);
        }

        [Test]
        public void Clear_AfterMoveAndRemove_LeavesCleanState()
        {
            _hash.Add(1, new float2(1, 1));
            _hash.Move(1, new float2(2, 2));
            _hash.Remove(1);

            _hash.Clear();

            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Clear_AfterManyElements_RemovesAllCells()
        {
            for (int i = 0; i < 1000; i++)
            {
                _hash.Add(i, new float2(i, i));
            }

            _hash.Clear();

            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }

        [Test]
        public void Clear_ThenAddSameItem_Works()
        {
            _hash.Add(1, new float2(1, 1));

            _hash.Clear();

            bool added =
                _hash.Add(1, new float2(5, 5));

            Assert.IsTrue(added);
            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Clear_EmptyHash_RemainsEmpty()
        {
            _hash.Clear();

            Assert.AreEqual(0, _hash.Count);
            Assert.AreEqual(0, _hash.CellCount);
        }
    }
}