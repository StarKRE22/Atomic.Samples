namespace Atomic.Infrastructure
{
    public interface IUpdater<in T>
    {
        void Update(T obj, float deltaTime);
    }
}