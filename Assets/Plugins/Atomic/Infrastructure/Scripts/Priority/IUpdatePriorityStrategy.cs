namespace Atomic.Infrastructure
{
    public interface IUpdatePriorityStrategy<in T>
    {
        UpdatePriority EvaluatePriority(T obj);
    }
}