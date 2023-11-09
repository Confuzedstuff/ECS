using System;

namespace ECSDomain;
public readonly struct EntityIndex : IEquatable<EntityIndex>
{
    public bool Equals(EntityIndex other) => Index == other.Index && Generation == other.Generation;

    public override bool Equals(object? obj) => obj is EntityIndex other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Index, Generation);

    public readonly int Index;
    public readonly int Generation;

    public EntityIndex(in int index, in int generation)
    {
        Index = index;
        Generation = generation;
    }

    public override string ToString() => $"{Index}:{Generation}";
}