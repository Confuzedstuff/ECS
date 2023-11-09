namespace ECSDomain;
public readonly struct GEntity
{
    public static readonly GEntity Null = new(Statics.Null, new GIndex());
    public readonly GlobalId EntityArchetype;
    public readonly GIndex GIndex;

    public GEntity(in GlobalId entityArchetype,
                   in GIndex gIndex)
    {
        EntityArchetype = entityArchetype;
        GIndex = gIndex;
    }

    public override string ToString() => $"{EntityArchetype} {GIndex}";
}