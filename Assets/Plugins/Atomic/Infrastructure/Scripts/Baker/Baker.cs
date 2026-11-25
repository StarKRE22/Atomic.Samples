#if UNITY_5_3_OR_NEWER

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atomic.Infrastructure
{
    public abstract class Baker<T, TArgs> : MonoBehaviour where TArgs : IArgs
    {
        public T Bake(TArgs args)
        {
            T entity = this.BakeInternal(args);
            this.Release();
            return entity;
        }

        protected virtual void Release() =>
            Destroy(this.gameObject);

        protected abstract T BakeInternal(TArgs args);

        /// <summary>
        /// Finds all <see cref="Baker{T,TArgs}"/> components in the scene and bakes them into entities.
        /// All corresponding GameObjects will be destroyed after baking.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive objects in the search.</param>
        /// <returns>Array of baked entities.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] BakeAll(TArgs args, bool includeInactive = true)
        {
            FindObjectsInactive include = includeInactive
                ? FindObjectsInactive.Include
                : FindObjectsInactive.Exclude;

            var bakers = FindObjectsByType<Baker<T, TArgs>>(include, FindObjectsSortMode.None);
            int count = bakers.Length;
            T[] entities = new T[count];

            for (int i = 0; i < count; i++)
            {
                var baker = bakers[i];
                if (includeInactive || baker.gameObject.activeInHierarchy)
                {
                    T entity = baker.Bake(args);
                    entities[i] = entity;
                }
            }

            return entities;
        }

        /// <summary>
        /// Collects entities from all <see cref="Baker{T,TArgs}"/> components in the scene
        /// and adds them to the specified <paramref name="destination"/> collection.
        /// </summary>
        /// <typeparam name="T">The type of entity created by the bakers.</typeparam>
        /// <param name="destination">
        /// The collection where all baked entities will be stored.  
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="includeInactive">
        /// Whether to include bakers attached to inactive GameObjects.  
        /// If <c>false</c>, only active bakers are considered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="destination"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// This method finds all <see cref="Baker{T,TArgs}"/> instances in the scene and
        /// invokes their <c>Create</c> method to generate entities.  
        /// The resulting entities are then added to <paramref name="destination"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BakeAll(TArgs args, ICollection<T> destination, bool includeInactive = true)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            FindObjectsInactive include = includeInactive
                ? FindObjectsInactive.Include
                : FindObjectsInactive.Exclude;

            var bakers = FindObjectsByType<Baker<T, TArgs>>(include, FindObjectsSortMode.None);

            int count = bakers.Length;
            for (int i = 0; i < count; i++)
            {
                var baker = bakers[i];
                if (includeInactive || baker.gameObject.activeInHierarchy)
                {
                    T entity = baker.Bake(args);
                    destination.Add(entity);
                }
            }
        }

        /// <summary>
        /// Bakes all <see cref="Baker{T,TArgs}"/>s in a specific <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The scene whose root objects should be searched.</param>
        /// <returns>List of baked entities.</returns>
        public static List<T> Bake(TArgs args, Scene scene, bool includeInactive = true)
        {
            var result = new List<T>();
            GameObject[] rootObjects = scene.GetRootGameObjects();

            for (int i = 0, rootCount = rootObjects.Length; i < rootCount; i++)
            {
                GameObject rootObject = rootObjects[i];
                var bakers = rootObject.GetComponentsInChildren<Baker<T, TArgs>>(includeInactive);
                for (int j = 0, bakerCount = bakers.Length; j < bakerCount; j++)
                {
                    var baker = bakers[j];
                    if (includeInactive || baker.gameObject.activeInHierarchy)
                    {
                        T entity = baker.Bake(args);
                        result.Add(entity);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Bakes all <see cref="Baker{T,TArgs}"/>s in a specific <see cref="Scene"/> and adds them to the provided collection.
        /// </summary>
        /// <param name="scene">The scene whose root objects should be searched.</param>
        /// <param name="results">The collection where baked entities will be added.</param>
        /// <param name="includeInactive">Whether to include inactive objects in the search.</param>
        public static void Bake(TArgs args, Scene scene, ICollection<T> results, bool includeInactive = true)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            GameObject[] objects = scene.GetRootGameObjects();
            for (int i = 0, objectCount = objects.Length; i < objectCount; i++)
            {
                GameObject go = objects[i];
                var bakers = go.GetComponentsInChildren<Baker<T, TArgs>>(includeInactive);

                for (int j = 0, bakerCount = bakers.Length; j < bakerCount; j++)
                {
                    var baker = bakers[j];
                    if (includeInactive || baker.gameObject.activeInHierarchy)
                    {
                        T entity = baker.Bake(args);
                        results.Add(entity);
                    }
                }
            }
        }


        /// <summary>
        /// Bakes all <see cref="Baker{T,TArgs}"/> components attached to or under the specified GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to search.</param>
        /// <returns>Array of baked entities.</returns>
        public static T[] Bake(TArgs args, GameObject gameObject, bool includeInactive = true)
        {
            var bakers = gameObject.GetComponentsInChildren<Baker<T, TArgs>>(includeInactive);
            int count = bakers.Length;
            T[] entities = new T[count];

            for (int i = 0; i < count; i++)
            {
                var baker = bakers[i];
                if (!includeInactive && !baker.gameObject.activeInHierarchy)
                    continue;

                T entity = baker.Bake(args);
                entities[i] = entity;
            }

            return entities;
        }

        /// <summary>
        /// Bakes all <see cref="Baker{T,TArgs}"/> components attached to or under the specified GameObject
        /// and adds them to the provided collection.
        /// </summary>
        /// <param name="gameObject">The GameObject to search.</param>
        /// <param name="results">The collection where baked entities will be added.</param>
        /// <param name="includeInactive">Whether to include inactive objects in the search.</param>
        public static void Bake(TArgs args, GameObject gameObject, ICollection<T> results, bool includeInactive = true)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Baker<T, TArgs>[] bakers = gameObject.GetComponentsInChildren<Baker<T, TArgs>>(includeInactive);
            for (int i = 0, count = bakers.Length; i < count; i++)
            {
                Baker<T, TArgs> baker = bakers[i];
                if (includeInactive || baker.gameObject.activeInHierarchy)
                {
                    T entity = baker.Bake(args);
                    results.Add(entity);
                }
            }
        }
    }
}
#endif