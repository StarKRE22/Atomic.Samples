namespace Atomic.Infrastructure
{
    public interface IFactory<out T, in TArgs> where TArgs : IArgs
    {
        T Create(TArgs args);
    }
}