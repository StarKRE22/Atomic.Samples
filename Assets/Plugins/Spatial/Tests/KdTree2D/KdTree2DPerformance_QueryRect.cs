using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class KdTree2DPerformance_QueryRect
    {
        private const int Count = 10_000;

        private static readonly Vector2 RectSize = new(100, 100);

        private KdTree2D<int> _tree;
        private Rect[] _queries;
        private int[] _results;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tree = new KdTree2D<int>();

            _queries = new Rect[Count];
            _results = new int[Count];

            int id = 0;

            for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++)
            {
                Vector2 point = new Vector2(x * 10f, y * 10f);

                _tree.Add(id, point);

                _queries[id] = new Rect(
                    point.x - RectSize.x / 2,
                    point.y - RectSize.y / 2,
                    RectSize.x,
                    RectSize.y
                );

                id++;
            }
        }

        [Test, Performance]
        public void QueryRect_10K()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++)
                        _tree.QueryRect(_queries[i], _results);
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }
    }
}