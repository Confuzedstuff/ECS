
using System.Runtime.CompilerServices;

public readonly struct None
{
}

public static class Option
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(in T t) where T : struct => new(t);

    public static readonly None None = new();
}

public readonly struct Option<T>
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

    private static readonly Option<T> None = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(T value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(in None none) => None;
}