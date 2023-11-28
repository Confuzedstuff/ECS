namespace ECSDomain;
public readonly struct EntityIndex(in int index, in int generation) : IEquatable<EntityIndex>
{
    public readonly int Index = index;
    public readonly int Generation = generation;
    public bool Equals(EntityIndex other) => Index == other.Index && Generation == other.Generation;
    public override bool Equals(object? obj) => obj is EntityIndex other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Index, Generation);
    public override string ToString() => $"{Index}:{Generation}";
}