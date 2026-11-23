using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreePerformance_QueryCircle
    {
        private const int Count = 10_000;
        private const float Radius = 200f;

        private QuadTree<int> _tree;
        private Vector2[] _queries;
        private int[] _results;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tree = new QuadTree<int>(
                Vector2.zero,
                new Vector2(1000f, 1000f),
                capacity: 128,
                maxDepth: 8
            );

            _queries = new Vector2[Count];
            _results = new int[Count];

            int id = 0;

            for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++)
            {
                Vector2 point = new Vector2(x * 10f, y * 10f);

                _tree.Add(id, point);
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
                        _tree.QueryCircle(_queries[i], Radius, _results);
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }
    }
}