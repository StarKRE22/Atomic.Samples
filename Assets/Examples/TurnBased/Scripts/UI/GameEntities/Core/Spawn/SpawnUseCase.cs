using Atomic.Entities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Gameplay;
using UnityEngine;

namespace Game.UI
{
    public static class SpawnUseCase
    {
        public static async UniTask SpawnEntityView(this IUIContext context, IGameEntity entity, Vector2Int position)
        {
            GameEntityViewStore viewStore = context.GetEntityCollectionView();
            GameEntityView view = viewStore.Add(entity);
            
            Transform transform = view.Model.GetTransform();
            transform.position = context.GetWorldPosition(position);
            
            await transform
                .DOScale(Vector3.one, 0.5f)
                .ChangeStartValue(Vector3.zero)
                .SetLink(transform.gameObject)
                .AsyncWaitForCompletion();
        }
    }
}