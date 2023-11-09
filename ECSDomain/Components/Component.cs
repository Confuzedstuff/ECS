using System;
using System.Runtime.CompilerServices;

public abstract class Component // TODO rename to Buffer
{
    public abstract Type GetComponentType();
    public abstract void Spawn();
    public abstract void DeSpawn(in int index);
}

public class Component<T> : Component
    where T : struct
{
    public int nextIndex = 0; // guess this means length
    protected T[] elements = new T[1];

    public virtual ref T Get(int index) => ref elements[index];
    public virtual void Set(int index, in T t) => elements[index] = t;

    public virtual ref readonly T GetReadOnly(in int index) => ref elements[index];

    public Span<T> Elements => new(elements, 0, nextIndex);
    public ref readonly T Last => ref elements[nextIndex - 1];

    public override Type GetComponentType() => typeof(T);

    public override void Spawn()
    {
        if (nextIndex == elements.Length)
        {
            Array.Resize(ref elements, (int) (elements.Length * 1.5) + 1);
        }

        nextIndex++;
    }

    public override void DeSpawn(in int index)
    {
        nextIndex--;
        elements[index] = elements[nextIndex];
        elements[nextIndex] = default; // TODO ?
    }
}