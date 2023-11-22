using System;
using ECSDomain;

public abstract class Query
{
    protected int archIndex;
    protected int index;
    protected Archetype currentArch;
    protected Archetype[] arches;

    public Query()
    {
        LookupArches();
        Reset();
    }

    public void Reset()
    {
        archIndex = -1;
        currentArch = null;
        NextArch();
        index = -1;
    }

    public bool Next()
    {
        index++;
        if (index == currentArch.Length)
        {
            bool hasNextArch;
            do
            {
                hasNextArch = NextArch();
                if (hasNextArch && currentArch.Length != 0)
                {
                    return true;
                }
            } while (hasNextArch);

            return false;
        }

        return true;
    }

    private bool NextArch()
    {
        index = 0;
        archIndex++;
        if (archIndex == arches.Length) return false;
        currentArch = arches[archIndex];
        LookupArchComponents();
        return true;
    }

    protected abstract Type[] GetWithTypes();
    protected abstract void LookupArchComponents();

    public void LookupArches()
    {
        var query = new ArchQuery();
        foreach (var withType in GetWithTypes())
        {
            query.element.Add(With.Create(withType));
        }

        arches = ECS.Instance.GetValidArches(query).ToArray();
    }
}