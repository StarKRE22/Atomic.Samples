using System;
using UnityEngine;

namespace Atomic.Infrastructure
{
    [CreateAssetMenu(
        fileName = "AimdBudgetStrategy",
        menuName = "Atomic/Infrastructure/Adaptive Performance/New AimdBudgetStrategy"
    )]
    public sealed class AimdBudgetStrategy : ScriptableBudgetStrategy
    {
        [Min(1)]
        [SerializeField]
        private int _targetFPS = 60;

        [Range(0f, 0.5f)]
        [SerializeField]
        private float _tolerance = 0.1f;

        [Min(0)]
        [SerializeField]
        private int _minBudget = 32;

        [Min(1)]
        [SerializeField]
        private int _maxBudget = 2048;

        [Min(2)]
        [SerializeField]
        private int _scaleDown = 2;

        [Min(1)]
        [SerializeField]
        private int _stepUp = 64;
        
        public override int Adjust(int currentBudget, float cpuTime)
        {
            int min = Mathf.Max(0, _minBudget);
            int max = Mathf.Max(min, _maxBudget);

            int budget = Mathf.Clamp(currentBudget, min, max);

            float target = 1000f / Mathf.Max(1, _targetFPS);
            float tolerance = Mathf.Clamp(_tolerance, 0f, 0.5f);

            float lowerBound = target * (1f - tolerance);
            float upperBound = target * (1f + tolerance);

            if (cpuTime > upperBound)
            {
                int divisor = Mathf.Max(2, _scaleDown);
                return Mathf.Max(min, budget / divisor);
            }

            if (cpuTime < lowerBound)
            {
                int step = Mathf.Max(1, _stepUp);
                return (int)Math.Min(max, (long)budget + step);
            }

            return budget;
        }
    }
}