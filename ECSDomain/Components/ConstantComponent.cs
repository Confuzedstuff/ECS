namespace ECSDomain;
public sealed class ConstantComponent<T> : Component<T> where T : struct
{
    //TODO set size 1

    public ConstantComponent(in T t)
    {
        nextIndex++;
        elements[0] = t;
    }

    public override ref T Get(int index) => ref elements[0];
    public override ref readonly T GetReadOnly(in int index) => ref elements[0];
    public override void Set(int index, in T t) => elements[0] = t;

    public override void Spawn()
    {
    }

    public override void DeSpawn(in int index)
    {
    }
}