using System;

namespace ECSDomain;
public readonly struct GIndex // TODO EntityIndex
    : IEquatable<GIndex>
{
    public bool Equals(GIndex other) => Index == other.Index && Generation == other.Generation;

    public override bool Equals(object? obj) => obj is GIndex other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Index, Generation);

    public readonly int Index;
    public readonly int Generation;

    public GIndex(in int index, in int generation)
    {
        Index = index;
        Generation = generation;
    }

    public override string ToString() => $"{Index}:{Generation}";
}