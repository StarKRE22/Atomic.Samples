namespace Atomic.Infrastructure
{
    public interface IPriorityStrategy<in T>
    {
        Priority Evaluate(T obj);
    }
}