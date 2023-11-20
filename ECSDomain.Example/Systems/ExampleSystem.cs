using ECSDomain;


public partial class ExampleSystem : ECSSystem
{
    public void UpdateEntity(
        ref RocketComponent rocketComponent, // use `ref` for mutable access
        in HealthComponent healthComponent, // `in` for readonly access
        ref TransformComponent transformComponent
    )
    {
        // this will be called once per frame for each entity in all archetypes that mach the `UpdateEntity` method parameters
    }
}