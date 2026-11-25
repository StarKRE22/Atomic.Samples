#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace Atomic.Entities
{
    /// <summary>
    /// A non-generic alias for <see cref="MonoEntityManager{E}"/>.
    /// Represents a Unity scene-bound entity world operating on base <see cref="MonoEntity"/> types.
    /// </summary>
    /// <remarks>
    /// Use this when you don't need to specialize the world with a custom entity type.
    /// Useful for simple scenarios where only <see cref="MonoEntity"/> is involved.
    /// </remarks>
    [AddComponentMenu("Atomic/Entities/Entity World")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/Worlds/MonoEntityWorld.md")]
    public class MonoEntityManager : MonoEntityManager<IEntity>
    {
        public static MonoEntityManager Create(
            string name = null,
            bool scanEntities = true,
            bool useUnityLifecycle = true
        )
        {
            GameObject go = new GameObject();
            go.SetActive(false);

            MonoEntityManager manager = go.AddComponent<MonoEntityManager>();
            manager.Name = name;
            manager.collectOnAwake = scanEntities;
            manager.useUnityLifecycle = useUnityLifecycle;

            go.SetActive(true);
            return manager;
        }
    }
}
#endif