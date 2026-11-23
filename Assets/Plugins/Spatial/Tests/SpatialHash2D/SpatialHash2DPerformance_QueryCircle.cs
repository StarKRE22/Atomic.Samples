using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class SpatialHash2DPerformance_QueryCircle
    {
        private const int Count = 10_000;
        private const float Radius = 200f;

        private SpatialHash2D<int> _hash;
        private Vector2[] _queries;
        private int[] _results;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _hash = new SpatialHash2D<int>(
                cellSize: 128,
                capacity: Count
            );

            _queries = new Vector2[Count];
            _results = new int[Count];

            int id = 0;

            for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++)
            {
                Vector2 point = new Vector2(x * 10f, y * 10f);

                _hash.Add(id, point);
                _queries[id] = point;

                id++;
            }
        }

        [Test, Performance]
        public void QueryCircle_10K()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++)
                        _hash.QueryCircle(_queries[i], Radius, _results);
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }
    }
}