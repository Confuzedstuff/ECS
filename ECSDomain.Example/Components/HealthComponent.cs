public struct HealthComponent
{
    public float MaxHealth;
    public float RemainingHealth;

    public HealthComponent(
        in float remainingHealth,
        in float maxHealth)
    {
        RemainingHealth = remainingHealth;
        MaxHealth = maxHealth;
    }
}