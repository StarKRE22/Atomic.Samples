#if UNITY_5_3_OR_NEWER
using UnityEngine;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// ScriptableObject responsible for automatically spawning a set of
    /// <see cref="MonoEntity"/> prefabs when a specific scene is loaded.
    /// </summary>
    /// <remarks>
    /// Scene matching is performed using a regular expression.
    /// Entity spawning can occur either before or after the scene is fully loaded.
    /// </remarks>
    [CreateAssetMenu(
        fileName = "EntityBootstrapper",
        menuName = "Atomic/Entities/EntityBootstrapper"
    )]
    public class ScriptableEntityBootstrapper : ScriptableBootstrapper
    {
#if ODIN_INSPECTOR
        [GUIColor(0f, 0.83f, 1f)]
#endif
        [SerializeField]
        private bool isEnabled = true;
        
#if ODIN_INSPECTOR
        [EnableIf(nameof(isEnabled))]
#endif
        [Tooltip(
            "Regular expression used to determine whether the current scene should trigger this bootstrap. " +
            "If empty, the bootstrap applies to all scenes."
        )]
        [SerializeField]
        private string _sceneRegex;

#if ODIN_INSPECTOR
        [EnableIf(nameof(isEnabled))]
#endif
        [Tooltip("Defines when the bootstrap should perform entity spawning: before or after the scene load.")]
        [SerializeField]
        private Mode _mode = Mode.BeforeSceneLoad;

#if ODIN_INSPECTOR
        [EnableIf(nameof(isEnabled))]
#endif
        [Space]
        [Tooltip("List of Entity prefabs to spawn in the scene.")]
        [SerializeField]
        private MonoEntity[] _entityPrefabs;

        protected override void BootstrapInternal(Scene scene)
        {
            for (int i = 0, count = _entityPrefabs.Length; i < count; i++)
            {
                MonoEntity prefab = _entityPrefabs[i];
                MonoEntity.Create(prefab, scene);
            }
        }
    }
}
#endif