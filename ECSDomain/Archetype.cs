using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECSDomain.Messages;

namespace ECSDomain;
public abstract class Archetype
{
    public readonly GlobalId GlobalId;
    public readonly Component<EntityIndex> indexes;
    private readonly Mapping mapping;
    private readonly List<Component> components = new();

    protected Archetype(in GlobalId globalId)
    {
        GlobalId = globalId;
        var ind = new IndexComponent();

        indexes = ind;
        components.Add(indexes);
        mapping = new Mapping(ind);
        this.InjectMessaging();
    }

    public void Init()
    {
        AutoRegisterComponents();
    }

    private void AutoRegisterComponents()
    {
        var currentType = GetType();
        var fields = currentType.GetAllFields();
        var readers = fields.Where(x => typeof(Component).IsAssignableFrom(x.FieldType));
        var fun = currentType.GetMethod(nameof(RegisterComponent), BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var fieldInfo in readers)
        {
            var value = fieldInfo.GetValue(this);
            if (value is not null) continue; // handles indexes field
            var genericType = fieldInfo.FieldType.GenericTypeArguments.First();
            var fungen = fun.MakeGenericMethod(genericType);
            var instance = fungen.Invoke(this, Array.Empty<object>());
            fieldInfo.SetValue(this, instance);
        }
    }

    protected Component<T> RegisterComponent<T>() where T : struct
    {
        var component = new Component<T>();
        return RegisterComponentInstance(component);
    }

    protected Component<T> RegisterComponentInstance<T>(Component<T> component) where T : struct
    {
        components.Add(component);
        return component;
    }


    public virtual EntityIndex BaseSpawn(out int actualIndex)
    {
        foreach (var component in components)
        {
            component.Spawn();
        }

        mapping.Spawn();
        actualIndex = indexes.nextIndex - 1;
        return indexes.Last;
    }

    public void Despawn(in EntityIndex toDespawn)
    {
        if (ResolveActualEntityIndex(toDespawn, out var actualIndex))
        {
            mapping.DeSpawn(toDespawn.Index);
            foreach (var component in components)
            {
                component.DeSpawn(actualIndex);
            }
        }
        else
        {
            Console.WriteLine($"Tried to despawn entity {toDespawn}, ai: {actualIndex}"); // TODO logger
        }
    }

    public bool HasComponent<T>() where T : struct
    {
        var component = GetComponent<T>();

        return component != null;
    }

    public Component<T> GetComponent<T>() where T : struct
    {
        foreach (var component in components)
        {
            if (component.GetComponentType() == typeof(T))
            {
                return component as Component<T>;
            }
        }

        return null;
    }

    protected bool ResolveActualEntityIndex(in EntityIndex index, out int actualIndex)
    {
        actualIndex = mapping.Get(index.Index);
        ref readonly var curr = ref indexes.GetReadOnly(actualIndex);
        return curr.Generation == index.Generation;
    }

    public int Length => indexes.nextIndex;
    
    public ref T GetEntityComponents<T>(in EntityIndex index, out bool entityAlive) where T : struct //TODO partial
    {
        entityAlive = ResolveActualEntityIndex(index, out var actualIndex);
        var component = GetComponent<T>();
        if (entityAlive)
        {
            return ref component.Get(actualIndex);
        }

        return ref component.Get(0);
    }
}