namespace ECSDomain;
public abstract class ECSSystem
{
    protected List<Archetype> arches;
    public float delta;

    public ECS ecs { get; private set; }

    public void OnRegister(ECS ecs)
    {
        this.ecs = ecs;
        ecs.InjectAll(this);
    }

    public virtual void Update()
    {
    }

    public virtual void PostExecute(in float delta)
    {
    }

    public abstract void Execute(in float delta);


    public abstract Type[] GetWithTypes();


    public virtual void Init()
    {
    }

    public ArchQuery GetQuery()
    {
        var query = new ArchQuery();
        foreach (var withType in GetWithTypes())
        {
            query.element.Add(With.Create(withType));
        }

        return query;
    }

    public void SetArches(List<Archetype> validItems)
    {
        arches = validItems;
    }
}