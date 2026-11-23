using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Modules.Spatial
{
    [NativeContainer]
    public struct NativeSpatialHash2D<T> : IDisposable where T : unmanaged, IEquatable<T>
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;

        private static readonly SharedStatic<int> s_staticSafetyId =
            SharedStatic<int>.GetOrCreate<NativeSpatialHash2D<T>>();
#endif

        private UnsafeSpatialHash2D<T> _hash;

        public int Count
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return _hash.Count;
            }
        }
        public float CellSize
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return _hash.CellSize;
            }
        }
        public int CellCount
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return _hash.CellCount;
            }
        }
        public bool IsCreated => _hash.IsCreated;

        public NativeSpatialHash2D(
            Allocator allocator,
            float cellSize,
            int initialCapacity = 128,
            int initialCellCapacity = 4
        )
        {
            _hash = new UnsafeSpatialHash2D<T>(allocator, cellSize, initialCapacity, initialCellCapacity);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = CollectionHelper.CreateSafetyHandle(allocator);

            CollectionHelper.SetStaticSafetyId<NativeSpatialHash2D<T>>(ref m_Safety, ref s_staticSafetyId.Data);

            if (UnsafeUtility.IsNativeContainerType<T>())
                AtomicSafetyHandle.SetNestedContainer(m_Safety, true);
#endif
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif
            _hash.Dispose();
        }

        public bool Add(in T item, float2 position)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return _hash.Add(item, position);
        }

        public bool Remove(in T item)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

            return _hash.Remove(item);
        }

        public bool Move(T item, float2 position)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return _hash.Move(item, position);
        }

        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            _hash.Clear();
        }

        public bool QueryClosestInRadius(float2 center, float radius, out float2 closestPoint, out T closest)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return _hash.QueryClosestInRadius(center, radius, out closestPoint, out closest);
        }

        public int GetCount(long key)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return _hash.GetCount(key);
        }

        public NativeArray<long> GetKeys(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return _hash.GetKeys(allocator);
        }
    }
}