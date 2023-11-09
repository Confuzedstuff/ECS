namespace ECSDomain.Tests;
internal partial class TestEcsSystem : ECSSystem
{
    public void Update(TestPlanetId testPlanetId, TestTransfrom transform)
    {
        //testa
        Console.WriteLine($"{testPlanetId} {transform}");
    }
}