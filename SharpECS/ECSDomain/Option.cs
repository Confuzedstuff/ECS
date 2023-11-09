using System.Runtime.CompilerServices;

public sealed class Option
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(in T t) where T : struct => new(t);
}

public readonly struct Option<T>
    where T : struct
{
    public readonly bool HasValue;
    public readonly T Value;

    public Option(in T value)
    {
        HasValue = true;
        Value = value;
    }

    public Option()
    {
        HasValue = false;
        Value = default;
    }

    public static readonly Option<T> None = new();
}