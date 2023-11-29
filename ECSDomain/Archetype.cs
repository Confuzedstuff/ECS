using System.Collections.Frozen;
using System.Reflection;

namespace ECSDomain;
public abstract partial class Archetype
{
    public readonly GlobalId GlobalId;
    public readonly Component<EntityIndex> indexes;
    private readonly Mapping mapping;
    private FrozenDictionary<Type, Component> components;
    private List<Component> initcomponents = new();

    protected Archetype(in GlobalId globalId)
    {
        GlobalId = globalId;

        indexes = new IndexComponent();
        initcomponents.Add(indexes);
        mapping = new Mapping((IndexComponent) indexes);
    }

    public void Init(ECS ecs)
    {
        ecs.InjectAll(this);
        AutoRegisterComponents();
        components = initcomponents
            .ToDictionary(k => k.GetComponentType(), v => v)
            .ToFrozenDictionary();
        initcomponents = null;
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

    protected Component<T> RegisterComponent<T>()
    {
        var component = new Component<T>();
        return RegisterComponentInstance(component);
    }

    protected Component<T> RegisterComponentInstance<T>(Component<T> component)
    {
        initcomponents.Add(component);
        return component;
    }


    protected virtual EntityIndex BaseSpawn(out int actualIndex)
    {
        foreach (var component in components.Values)
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
            foreach (var component in components.Values)
            {
                component.DeSpawn(actualIndex);
            }
        }
        else
        {
            Console.WriteLine($"Tried to despawn entity {toDespawn}, ai: {actualIndex}"); // TODO logger
        }
    }

    public bool HasComponent<T>() => components.TryGetValue(typeof(T), out _);

    public Component<T> GetComponent<T>() => (Component<T>) components[typeof(T)];

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

    protected ref T GenericGet<T>(Entity entity, Component<T> component, out bool entityAlive)
    {
        entityAlive = ResolveActualEntityIndex(entity.EntityIndex, out var actualIndex);
        if (entityAlive)
        {
            return ref component.Get(actualIndex);
        }

        return ref component.Get(0);
    }
}