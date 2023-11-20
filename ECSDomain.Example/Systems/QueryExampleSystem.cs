using ECSDomain;

public partial class RocketQuery : Query // this generates a query that can be used to iterate over all archetypes that have matching components 
{
    public RocketQuery(ref RocketComponent rocketComponent, 
                       in TransformComponent transformComponent)
    {
        
    }
}

public partial class QueryExampleSystem : ECSSystem
{
    private readonly RocketQuery rocketQuery = new(); // instantiate query only once in system 
    public void UpdateEntity(
        in HealthComponent healthComponent, 
        ref TransformComponent transformComponent
    )
    {
        rocketQuery.Reset(); // since each iteration uses the same instance of the query it needs to be reset before iterating again
        while (rocketQuery.Next()) 
        {
            //this loops over all entities in this query
            rocketQuery.rocketComponent.Speed += 1;
            rocketQuery.transformComponent.Position = transformComponent.Position;
        }
    }
}