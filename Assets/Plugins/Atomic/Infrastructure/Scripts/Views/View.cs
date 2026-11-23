#if UNITY_5_3_OR_NEWER
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if ENABLE_PROFILER
using Unity.Profiling;
#endif

namespace Atomic.Infrastructure
{
    public abstract class View<TModel> : MonoBehaviour where TModel : class
    {
#if ENABLE_PROFILER
        private static readonly ProfilerMarker s_activateMarker = new($"View<{typeof(TModel).Name}>.Activate");
        private static readonly ProfilerMarker s_deactivateMarker = new($"View<{typeof(TModel).Name}>.Deactivate");
#endif

        /// <summary>
        /// Gets the entity currently associated with this view.
        /// </summary>
        public TModel Model => _model;

#if ODIN_INSPECTOR
        [Title("Debug")]
        [PropertyOrder(1000)]
        [ShowInInspector]
        [HideInEditorMode]
#endif
        private TModel _model;

        /// <summary>
        /// Gets a value indicating whether the view is currently visible (i.e., has an entity assigned).
        /// </summary>
        public bool IsActive => _model != null;

        /// <summary>
        /// Displays the view and associates it with the specified entity.
        /// </summary>
        /// <param name="model">The entity to associate with and display through this view.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="model"/> is null.</exception>
        public void Show(TModel model)
        {
#if ENABLE_PROFILER
            using (s_activateMarker.Auto())
#endif
            {
                this.Hide();
                _model = model ?? throw new ArgumentNullException(nameof(model));

                this.name = this.FormateName(model);
                this.OnShow(model);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual string FormateName(TModel entity) => entity.ToString();

        /// <summary>
        /// Hides the view and removes its association with the current entity.
        /// </summary>
        public void Hide()
        {
#if ENABLE_PROFILER
            using (s_deactivateMarker.Auto())
#endif
            {
                if (_model == null)
                    return;

                this.OnHide(_model);
                _model = null;
            }
        }

        /// <summary>
        /// Called when the view is shown.
        /// Override this method to add custom logic when an entity is assigned and the view becomes visible.
        /// </summary>
        /// <param name="model">The entity being displayed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnShow(TModel model)
        {
        }

        /// <summary>
        /// Called when the view is hidden.
        /// Override this method to add custom logic when the entity is removed and the view becomes invisible.
        /// </summary>
        /// <param name="model">The entity that was being displayed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnHide(TModel model)
        {
        }

        #region Gizmos

        [Header("Gizmos")]
#if ODIN_INSPECTOR
        [DisableInPlayMode]
        [PropertyOrder(5)]
#endif
        [Tooltip("If true, gizmos will be drawn only when the object is selected.")]
        [SerializeField]
        private bool _onlySelectedGizmos;

        [Tooltip("If true, gizmos will be drawn only in Edit Mode, even during play mode.")]
#if ODIN_INSPECTOR
        [DisableInPlayMode]
        [PropertyOrder(5)]
#endif
        [SerializeField]
        private bool _onlyEditModeGizmos;

        /// <summary>
        /// Unity callback invoked to draw gizmos for this component.
        /// When <see cref="_onlySelectedGizmos"/> is false, defers drawing to <see cref="OnDrawGizmosSelected"/>.
        /// </summary>
        private protected virtual void OnDrawGizmos()
        {
            if (!_onlySelectedGizmos)
                this.OnDrawGizmosSelected();
        }

        /// <summary>
        /// Unity callback invoked when the object is selected.
        /// Draws custom gizmos using behaviours that implement <see cref="IEntityGizmos{E}"/>.
        /// </summary>
        private protected virtual void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying && _onlyEditModeGizmos)
                return;
#endif
            this.OnGizmosDraw();
        }

        /// <summary>
        /// Internal method for drawing gizmos using entity behaviours that implement <see cref="IEntityGizmos{E}"/>.
        /// </summary>
        internal void OnGizmosDraw()
        {
            if (_model == null)
                return;

            try
            {
                this.DrawGizmos(_model);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Ops: detected exception in gizmos: {e.Message}");
            }
        }

        protected virtual void DrawGizmos(TModel model)
        {
        }

        #endregion
    }
}
#endif