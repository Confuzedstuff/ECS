using System;
using System.Collections.Generic;
using ECSDomain;

namespace ECSDomain;

public abstract class ECSSystem
{
    public ECS ecs; // TODO remove
    public void _Init()
    {
        this.RegisterMessaging();
        Init();
    }
    public virtual void Update(in float delta)
    {
        
    }
    
    public virtual void PostExecute(in float delta)
    {
        
    }
    public abstract void Execute(in float delta);

    protected List<Archetype> arches;

    public abstract Type[] GetWithTypes();

    public float delta;

    public abstract void Init();

    public QueryElements GetQuery()
    {
        var query = new QueryElements();
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