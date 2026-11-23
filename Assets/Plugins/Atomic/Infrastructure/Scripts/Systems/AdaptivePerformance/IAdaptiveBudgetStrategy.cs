namespace Atomic.Infrastructure
{
    public interface IAdaptiveBudgetStrategy
    {
        int Adjust(int currentBudget, float cpuTime);
    }
}