using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class UniformGrid2DPerformance_QueryCircle
    {
        private const int Count = 10_000;
        private const float Radius = 200f;

        private UniformGrid2D<int> _grid;
        private Vector2[] _queries;
        private int[] _results;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _grid = new UniformGrid2D<int>(
                sizeX: 1000,
                sizeY: 1000,
                cellSize: 128
            );

            _queries = new Vector2[Count];
            _results = new int[Count];

            int id = 0;

            for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++)
            {
                Vector2 point = new Vector2(x * 10f, y * 10f);

                _grid.Add(id, point);
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
                        _grid.QueryCircle(_queries[i], Radius, _results);
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }
    }
}