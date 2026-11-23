using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Modules.Spatial
{
    public class SpatialList2DPerformance_Move
    {
        private const int Count = 1000;

        private SpatialList2D<int> _list;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _list = new SpatialList2D<int>(Count);
        }

        [Test, Performance]
        public void Move_1K_Random()
        {
            Measure
                .Method(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _list.Move(i, new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f)));
                })
                .SetUp(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _list.Add(i, new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f)));
                })
                .CleanUp(_list.Clear)
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
                        _list.Move(id++, new Vector2(x + 1f, y + 1f));
                })
                .SetUp(() =>
                {
                    int id = 0;

                    for (int x = 0; x < 25; x++)
                    for (int y = 0; y < 40; y++)
                        _list.Add(id++, new Vector2(x, y));
                })
                .CleanUp(_list.Clear)
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
                        _list.Move(i, new Vector2(500f + Random.Range(-1f, 1f), 500f + Random.Range(-1f, 1f)));
                })
                .SetUp(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _list.Add(i, new Vector2(500f + Random.Range(-1f, 1f), 500f + Random.Range(-1f, 1f)));
                })
                .CleanUp(_list.Clear)
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
                        _list.Move(i, new Vector2(i * 0.1f, 600f));
                })
                .SetUp(() =>
                {
                    for (int i = 0; i < Count; i++) 
                        _list.Add(i, new Vector2(i * 0.1f, 500f));
                })
                .CleanUp(_list.Clear)
                .WarmupCount(3)
                .MeasurementCount(10)
                .Run();
        }
    }
}