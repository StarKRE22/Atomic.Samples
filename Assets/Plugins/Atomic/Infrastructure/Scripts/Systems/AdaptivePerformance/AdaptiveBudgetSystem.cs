
using System;
using UnityEngine;

namespace Atomic.Infrastructure
{
    public sealed class AdaptiveBudgetSystem : ISystem
    {
        [Serializable]
        public sealed class Settings
        {
            [Min(0.01f)]
            public float updateInterval = 0.25f;
        }

        private readonly IBudgetSystem _system;
        private readonly IAdaptiveBudgetStrategy _strategy;
        private readonly AdaptivePerformanceSource _performance;
        private readonly Settings _settings;

        private float _elapsed;

        public int Budget => _system.Budget;

        public AdaptiveBudgetSystem(
            IBudgetSystem system,
            IAdaptiveBudgetStrategy strategy,
            Settings settings
        )
        {
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _performance = AdaptivePerformanceSource.Instance;
        }

        public void Enable()
        {
            _system.Enable();
        }

        public void Disable()
        {
            _system.Disable();
        }

        public void Update(float deltaTime)
        {
            _elapsed += Time.unscaledDeltaTime;

            float interval = Mathf.Max(
                0.01f,
                _settings.updateInterval
            );

            if (_elapsed >= interval)
            {
                _elapsed %= interval;

                if (_performance.HasSample)
                {
                    AdjustBudget();
                }
            }

            _system.Update(deltaTime);
        }

        private void AdjustBudget()
        {
            int currentBudget = _system.Budget;

            int nextBudget = _strategy.Adjust(
                currentBudget,
                _performance.CPUTime
            );

            if (nextBudget != currentBudget)
            {
                _system.SetBudget(nextBudget);
            }
        }

        public void Dispose()
        {
            _system.Dispose();
        }
    }
}
