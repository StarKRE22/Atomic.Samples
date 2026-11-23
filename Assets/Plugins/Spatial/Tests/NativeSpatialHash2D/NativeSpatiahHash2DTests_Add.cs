using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace Modules.Spatial
{
    [TestFixture]
    public class NativeSpatialHash2DTests_Add
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
        public void Add_SingleElement_IncreasesCount()
        {
            bool result = _hash.Add(1, new float2(1, 1));

            Assert.IsTrue(result);
            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Add_MultipleElements_SameCell_UsesSingleCell()
        {
            _hash.Add(1, new float2(1.2f, 1.3f));
            _hash.Add(2, new float2(1.4f, 1.1f));
            _hash.Add(3, new float2(1.8f, 1.6f));

            Assert.AreEqual(3, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Add_DifferentCells_CreatesMultipleCells()
        {
            _hash.Add(1, new float2(0, 0));
            _hash.Add(2, new float2(10, 10));

            Assert.AreEqual(2, _hash.Count);
            Assert.AreEqual(2, _hash.CellCount);
        }

        [Test]
        public void Add_NegativeCoordinates_Works()
        {
            bool result =
                _hash.Add(1, new float2(-1.2f, -3.4f));

            Assert.IsTrue(result);
            Assert.AreEqual(1, _hash.Count);
            Assert.AreEqual(1, _hash.CellCount);
        }

        [Test]
        public void Add_DuplicateItem_ReturnsFalse()
        {
            bool first =
                _hash.Add(1, new float2(1, 1));

            bool second =
                _hash.Add(1, new float2(2, 2));

            Assert.IsTrue(first);
            Assert.IsFalse(second);

            Assert.AreEqual(1, _hash.Count);
        }

        [Test]
        public void Add_ItemsInNeighbourCells_CreateSeparateCells()
        {
            _hash.Add(1, new float2(0.5f, 0.5f));
            _hash.Add(2, new float2(1.5f, 0.5f));

            Assert.AreEqual(2, _hash.Count);
            Assert.AreEqual(2, _hash.CellCount);
        }

        [Test]
        public void Add_ManyItems_CountMatches()
        {
            const int count = 1000;

            for (int i = 0; i < count; i++)
            {
                _hash.Add(i, new float2(i, 0));
            }

            Assert.AreEqual(count, _hash.Count);
        }
    }
}