namespace ECSDomain.Tests;

internal class TestPlanetArch : Archetype
{
    public TestPlanetArch()
    {
        RegisterComponent<TestEntityType>();
        RegisterComponent<TestTransfrom>();
        RegisterComponent<TestPlanetId>();
    }
}

internal class TestUnitArch : Archetype
{
    public TestUnitArch()
    {
        RegisterComponent<TestEntityType>();
        RegisterComponent<TestTransfrom>();
    }
}