using System;
using UnityEngine;

namespace Atomic.Infrastructure
{
    public sealed class AimdBudgetStrategy : IAdaptiveBudgetStrategy
    {
        [Serializable]
        public sealed class Settings
        {
            [Min(1)]
            public int targetFPS = 60;

            [Range(0f, 0.5f)]
            public float tolerance = 0.1f;

            [Min(0)]
            public int minBudget = 32;

            [Min(1)]
            public int maxBudget = 2048;

            [Min(2)]
            public int scaleDown = 2;

            [Min(1)]
            public int stepUp = 64;
        }

        private readonly Settings _settings;

        public AimdBudgetStrategy(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public int Adjust(int currentBudget, float cpuTime)
        {
            int min = Mathf.Max(0, _settings.minBudget);
            int max = Mathf.Max(min, _settings.maxBudget);

            int budget = Mathf.Clamp(currentBudget, min, max);

            float target = 1000f / Mathf.Max(1, _settings.targetFPS);
            float tolerance = Mathf.Clamp(_settings.tolerance, 0f, 0.5f);

            float lowerBound = target * (1f - tolerance);
            float upperBound = target * (1f + tolerance);

            if (cpuTime > upperBound)
            {
                int divisor = Mathf.Max(2, _settings.scaleDown);
                return Mathf.Max(min, budget / divisor);
            }

            if (cpuTime < lowerBound)
            {
                int step = Mathf.Max(1, _settings.stepUp);
                return (int)Math.Min(max, (long)budget + step);
            }

            return budget;
        }
    }
}