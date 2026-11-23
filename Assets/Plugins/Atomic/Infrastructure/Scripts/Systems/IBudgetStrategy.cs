namespace Atomic.Infrastructure
{
    public interface IBudgetStrategy
    {
        int Adjust(int currentBudget, float cpuTime);
    }
}