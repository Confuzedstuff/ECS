namespace ECSDomain;
public readonly struct Entity
{
    public static readonly Entity Null = new(Statics.Null, new EntityIndex());
    public readonly GlobalId EntityArchetype;
    public readonly EntityIndex EntityIndex;

    public Entity(in GlobalId entityArchetype,
                   in EntityIndex entityIndex)
    {
        EntityArchetype = entityArchetype;
        EntityIndex = entityIndex;
    }

    public override string ToString() => $"{EntityArchetype} {EntityIndex}";
}