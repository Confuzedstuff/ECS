namespace ECSDomain.Tests;
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var ecs = ECS.Instance;
        var planetArch = ecs.GetArchitecture<TestPlanetArch>();
        var planet1 = planetArch.Spawn();
        var planet2 = planetArch.Spawn();
        var planet3 = planetArch.Spawn();
        planetArch.Despawn(planet2);

        Assert.That(planet1.Generation, Is.EqualTo(0));


        Assert.That(planet2.Index, Is.EqualTo(0));
    }

    [Test]
    public void TestQuery()
    {
        var ecs = ECS.Create();
        var planetArch = ecs.GetArchitecture<TestPlanetArch>();
        var unitArch = ecs.GetArchitecture<TestUnitArch>();
        var planet1 = planetArch.Spawn();
        var planet2 = planetArch.Spawn();
        var planet3 = planetArch.Spawn();
        
        var unit1 = unitArch.Spawn();
        var unit2 = unitArch.Spawn();
        var unit3 = unitArch.Spawn();
        
        planetArch.Despawn(planet2);

        
        Assert.That(planet1.Generation, Is.EqualTo(0));


        Assert.That(planet2.Index, Is.EqualTo(0));
    }
    
    [Test]
    public void TestSystem()
    {
        var ecs = ECS.Create();
        var planetArch = ecs.GetArchitecture<TestPlanetArch>();
        var unitArch = ecs.GetArchitecture<TestUnitArch>();
        
        var planet1 = planetArch.Spawn();
        var planet2 = planetArch.Spawn();
        var planet3 = planetArch.Spawn();
        
        var unit1 = unitArch.Spawn();
        var unit2 = unitArch.Spawn();
        var unit3 = unitArch.Spawn();
        
        planetArch.Despawn(planet2);

        var system = new TestEcsSystem();
        ecs.UpdateSystemArches(system);
        system.Execute();
    }
}