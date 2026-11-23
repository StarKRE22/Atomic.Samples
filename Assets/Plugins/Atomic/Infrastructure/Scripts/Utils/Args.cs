namespace Atomic.Infrastructure
{
    public interface IArgs
    {
    }

    public struct NoArgs : IArgs
    {
        public static readonly NoArgs Default = new();
    }

    public readonly struct Args<T> : IArgs
    {
        public readonly T value;

        public Args(T value)
        {
            this.value = value;
        }

        public static implicit operator Args<T>(T value) => new(value);
    }

    public readonly struct Args<T1, T2> : IArgs
    {
        public readonly T1 value1;
        public readonly T2 value2;

        public Args(T1 value1, T2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }
    }

    public readonly struct Args<T1, T2, T3> : IArgs
    {
        public readonly T1 value1;
        public readonly T2 value2;
        public readonly T3 value3;

        public Args(T1 value1, T2 value2, T3 value3)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
        }
    }

    public readonly struct Args<T1, T2, T3, T4> : IArgs
    {
        public readonly T1 value1;
        public readonly T2 value2;
        public readonly T3 value3;
        public readonly T4 value4;

        public Args(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
            this.value4 = value4;
        }
    }
}