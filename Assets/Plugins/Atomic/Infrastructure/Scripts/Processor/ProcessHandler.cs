namespace Atomic.Infrastructure
{
    public delegate void ProcessHandler<in T>(T obj, float deltaTime);
}