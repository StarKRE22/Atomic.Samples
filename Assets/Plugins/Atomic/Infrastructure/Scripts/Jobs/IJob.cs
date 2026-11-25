namespace Atomic.Infrastructure
{
    public interface IJob<in T>
    {
        void Execute(T obj, float deltaTime);
    }
}