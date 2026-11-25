namespace Atomic.Infrastructure
{
    public interface IUpdateStrategy<in T>
    {
        void Update(T obj, float deltaTime);
    }
}