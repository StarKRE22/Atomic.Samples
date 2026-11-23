#if UNITY_5_3_OR_NEWER
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Entities
{
    public abstract class ScriptableBootstrapper : ScriptableObject
    {
        public enum Mode
        {
            BeforeSceneLoad = 0,
            AfterSceneLoad = 1
        }

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

        protected virtual bool IsAvailable(Scene scene) =>
            string.IsNullOrEmpty(_sceneRegex) || Regex.IsMatch(scene.name, _sceneRegex);

        private async void Bootstrap(Scene scene)
        {
            if (_mode == Mode.AfterSceneLoad)
                while (!scene.isLoaded)
                    await Task.Yield();

            this.BootstrapInternal(scene);
        }

        protected abstract void BootstrapInternal(Scene scene);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            ScriptableBootstrapper[] bootstraps = Resources.LoadAll<ScriptableBootstrapper>(string.Empty);
            int count = bootstraps.Length;
            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                ScriptableBootstrapper bootstrapper = bootstraps[i];
                if (bootstrapper.isEnabled && bootstrapper.IsAvailable(activeScene))
                    bootstrapper.Bootstrap(activeScene);
            }
        }
    }
}
#endif