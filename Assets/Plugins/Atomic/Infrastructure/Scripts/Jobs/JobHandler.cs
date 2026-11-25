namespace Atomic.Infrastructure
{
    public delegate void JobHandler<in T>(T obj, float deltaTime);
}