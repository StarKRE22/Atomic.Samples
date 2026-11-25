namespace Atomic.Infrastructure
{
    public interface IBudgetSystem : ISystem
    {
        int Budget { get; }

        void SetBudget(int budget);
    }
}