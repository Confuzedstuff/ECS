public readonly partial struct GlobalId : IPvo<int>
{
    public static implicit operator GlobalId(int value) => new(value);
}