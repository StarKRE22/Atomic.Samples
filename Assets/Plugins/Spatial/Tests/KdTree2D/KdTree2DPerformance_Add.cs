using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class KdTree2DPerformance_Add
    {
        private const int Count = 1000;

        private KdTree2D<int> _tree;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tree = new KdTree2D<int>();
        }

        [Test, Performance]
        public void Add_1K_Random()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < Count; i++)
                        _tree.Add(i, new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f)));
                })
                .CleanUp(_tree.Clear)
                .WarmupCount(5)
                .MeasurementCount(20)
                .Run();
        }

        [Test, Performance]
        public void Add_1K_Grid()
        {
            Measure.Method(() =>
                {
                    int id = 0;
                    for (int x = 0; x < 25; x++)
                    for (int y = 0; y < 40; y++)
                        _tree.Add(id++, new Vector2(x, y));
                })
                .CleanUp(_tree.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }

        [Test, Performance]
        public void Add_1K_Cluster()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _tree.Add(i, new Vector2(500f + Random.Range(-1f, 1f), 500f + Random.Range(-1f, 1f)));
                })
                .CleanUp(_tree.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }

        [Test, Performance]
        public void Add_1K_Sorted()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _tree.Add(i, new Vector2(i, i));
                })
                .CleanUp(_tree.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }
    }
}