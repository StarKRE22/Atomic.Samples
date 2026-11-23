using Atomic.Entities;
using Game.Gameplay;

namespace Game.UI
{
    public static class HealthUseCase
    {
        public static void UpdateHealthBar(this GameEntityView entityView, int health)
        {
            ProgressBarPro healthBar = entityView.Model.GetHealthBar();
            healthBar.Value = (float) health / entityView.Model.GetMaxHealth().Value;
        }
    }
}