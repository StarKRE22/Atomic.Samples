using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class QuadTreePerformance_QueryClosest
    {
        private const int Count = 10_000;

        private QuadTree<int> _tree;
        private Vector2[] _queries;

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

            int id = 0;

            for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++)
            {
                Vector2 point = new Vector2(x * 10f, y * 10f);

                _tree.Add(id, point);
                _queries[id] = new Vector2(
                    x * 10f + 5f,
                    y * 10f + 5f
                );

                id++;
            }
        }

        [Test, Performance]
        public void QueryClosest_10K()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++)
                        _tree.QueryClosest(_queries[i], out _, out _);
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }
    }
}