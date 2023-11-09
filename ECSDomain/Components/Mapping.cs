using System;
using System.Collections.Generic;

namespace ECSDomain;
public class Mapping : Component
{
    private readonly Dictionary<int, int> mapping = new();
    private readonly IndexComponent indexComponent;

    public Mapping(IndexComponent indexComponent)
    {
       this.indexComponent = indexComponent;
    }

    public int Get(in int indexIndex) => mapping[indexIndex];
    public override Type GetComponentType() => typeof(int);

    public override void Spawn()
    {
        mapping[indexComponent.Last.Index] = indexComponent.nextIndex - 1;
    }
    public override void DeSpawn(in int index)
    {
        var actual = mapping[index];
        //mapping.Remove(index); // redundant?
        mapping[indexComponent.Last.Index] = actual;
    }
}