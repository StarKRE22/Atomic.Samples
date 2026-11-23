namespace Atomic.Infrastructure
{
    public interface IAspect<in T>
    {
        void Apply(T item);

        void Discard(T item);
    }
}