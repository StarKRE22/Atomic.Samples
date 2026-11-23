using UnityEngine;

namespace Atomic.Infrastructure
{
    public abstract class ScriptableBudgetStrategy : ScriptableObject, IBudgetStrategy
    {
        public abstract int Adjust(int currentBudget, float cpuTime);
    }
}