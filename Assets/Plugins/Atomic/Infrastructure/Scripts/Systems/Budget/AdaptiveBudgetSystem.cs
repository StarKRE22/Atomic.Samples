using System;
using UnityEngine;

namespace Atomic.Infrastructure
{
    public sealed class AdaptiveBudgetSystem : ISystem
    {
        public const float DEFAULT_INTERVAL = 0.1f;
        
        private readonly IBudgetSystem _system;
        private readonly IBudgetStrategy _strategy;
        private AdaptivePerformance _performance;

        private float _elapsed;

        public int Budget => _system.Budget;

        private readonly float _updateInterval;

        public AdaptiveBudgetSystem(
            IBudgetSystem system,
            IBudgetStrategy strategy,
            float updateInterval = DEFAULT_INTERVAL
        )
        {
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _updateInterval = updateInterval;
        }

        public void Enable()
        {
            _performance = AdaptivePerformance.Instance;
            _system.Enable();
        }

        public void Disable()
        {
            _system.Disable();
        }

        public void Update(float deltaTime)
        {
            this.UpdatePerformance();
            _system.Update(deltaTime);
        }

        private void UpdatePerformance()
        {
            _elapsed += Time.unscaledDeltaTime;

            float interval = Mathf.Max(0.01f, _updateInterval);
            if (_elapsed >= interval)
            {
                _elapsed %= interval;
                if (_performance.HasSample) 
                    this.AdjustBudget();
            }
        }

        private void AdjustBudget()
        {
            int currentBudget = _system.Budget;
            int nextBudget = _strategy.Adjust(currentBudget, _performance.CPUTime);
            if (nextBudget != currentBudget) 
                _system.SetBudget(nextBudget);
        }

        public void Dispose()
        {
            _system.Dispose();
        }
    }
}