namespace ECSDomain;
public readonly struct Entity(in GlobalId entityArchetype,
                              in EntityIndex entityIndex) : IEquatable<Entity>
{
    public static readonly Entity Null = new(Statics.Null, new EntityIndex());
    public readonly GlobalId EntityArchetype = entityArchetype;
    public readonly EntityIndex EntityIndex = entityIndex;

    public override string ToString() => $"{EntityArchetype} {EntityIndex}";

    public bool Equals(Entity other) => EntityArchetype.Equals(other.EntityArchetype) && EntityIndex.Equals(other.EntityIndex);

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(EntityArchetype, EntityIndex);

    public static bool operator ==(Entity left, Entity right) =>
        left.EntityArchetype == right.EntityArchetype
        && left.EntityIndex == right.EntityIndex;

    public static bool operator !=(Entity left, Entity right) => !(left == right);
}