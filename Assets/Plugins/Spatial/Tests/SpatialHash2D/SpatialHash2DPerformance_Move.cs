using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class SpatialHash2DPerformance_Move
    {
        private const int Count = 1000;

        private SpatialHash2D<int> _hash;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _hash = new SpatialHash2D<int>(cellSize: 128, capacity: Count);
        }

        [Test, Performance]
        public void Move_1K_Random()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _hash.Move(i, new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f)));
                })
                .SetUp(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _hash.Add(i, new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f)));
                })
                .CleanUp(_hash.Clear)
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }

        [Test, Performance]
        public void Move_1K_Grid()
        {
            Measure
                .Method(() =>
                {
                    int id = 0;

                    for (int x = 0; x < 25; x++)
                    for (int y = 0; y < 40; y++)
                        _hash.Move(id++, new Vector2(x + 1f, y + 1f));
                })
                .SetUp(() =>
                {
                    int id = 0;

                    for (int x = 0; x < 25; x++)
                    for (int y = 0; y < 40; y++)
                        _hash.Add(id++, new Vector2(x, y));
                })
                .CleanUp(_hash.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }

        [Test, Performance]
        public void Move_1K_Cluster()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _hash.Move(i, new Vector2(500f + Random.Range(-1f, 1f), 500f + Random.Range(-1f, 1f)));
                })
                .SetUp(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _hash.Add(i, new Vector2(500f + Random.Range(-1f, 1f), 500f + Random.Range(-1f, 1f)));
                })
                .CleanUp(_hash.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }

        [Test, Performance]
        public void Move_1K_SortedXAxis()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _hash.Move(i, new Vector2(i * 0.1f, 600f));
                })
                .SetUp(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _hash.Add(i, new Vector2(i * 0.1f, 500f));
                })
                .CleanUp(_hash.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }
    }
}