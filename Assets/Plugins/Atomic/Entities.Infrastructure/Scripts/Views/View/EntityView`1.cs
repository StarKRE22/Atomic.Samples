#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Atomic.Infrastructure;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if ENABLE_PROFILER
#endif

namespace Atomic.Entities
{
    /// <summary>
    /// Base class for all entity views.
    /// Provides core functionality for showing, hiding, and naming views bound to <see cref="IEntity"/>.
    /// </summary>
    /// <typeparam name="E">The type of <see cref="IEntity"/> associated with this view.</typeparam>
    [HelpURL("https://github.com/StarKRE22/Atomic/blob/main/Docs/Entities/UI/EntityView%601.md")]
    public abstract class EntityView<E> : View<E> where E : class, IEntity
    {
        private const string NAME_FORMAT = "{0}:{1}";
        /// <summary>
        /// List of installers that provide values and behaviors to the attached entity.
        /// </summary>
        [Header("Installing")]
        [Tooltip("Specify the installers that will put values and behaviours to an attached entity")]
        [SerializeField]
#if ODIN_INSPECTOR
        [PropertyOrder(5)]
        [DisableInPlayMode]
#endif
        internal List<MonoEntityInstaller> installers;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override string FormateName(E entity)
        {
            return string.Format(NAME_FORMAT, entity.Name, entity.InstanceID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnShow(E model)
        {
            if (this.installers != null)
            {
                for (int i = 0, count = this.installers.Count; i < count; i++)
                {
                    MonoEntityInstaller installer = this.installers[i];
                    if (installer)
                        installer.Install(model);
                    else
                        Debug.LogWarning(
                            $"EntityView<{typeof(E).Name}>: Oops! Detected null installer!",
                            this);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnHide(E model)
        {
            if (this.installers != null)
            {
                for (int i = 0, count = this.installers.Count; i < count; i++)
                {
                    MonoEntityInstaller installer = this.installers[i];
                    if (installer)
                        installer.Uninstall(model);
                    else
                        Debug.LogWarning($"EntityView<{typeof(E).Name}>: Oops! Detected null installer!", this);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void DrawGizmos(E model)
        {
            for (int i = 0, count = model.BehaviourCount; i < count; i++)
                if (model.GetBehaviourAt(i) is IEntityGizmos gizmos)
                    gizmos.DrawGizmos(model);
        }
    }
}
#endif