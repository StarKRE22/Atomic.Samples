namespace Atomic.Infrastructure
{
    public interface IProcessor<in T>
    {
        void Process(T obj, float deltaTime);
    }
}