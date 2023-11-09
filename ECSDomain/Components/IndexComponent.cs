namespace ECSDomain;
public class IndexComponent : Component<EntityIndex>
{
    public override void Spawn()
    {
        var expanded = nextIndex == elements.Length;

        base.Spawn();

        if (!expanded) return;
        for (var i = nextIndex - 1; i < elements.Length; i++)
        {
            elements[i] = new EntityIndex(i, 0);
        }
    }

    public override void DeSpawn(in int index)
    {
        nextIndex--;
        var temp = elements[index];
        elements[index] = elements[nextIndex];
        elements[nextIndex] = new EntityIndex(temp.Index, temp.Generation + 1);
    }
}