using ECSDomain;

public abstract class BaseEcsSystem : ECSSystem
{
    public void _Init()
    {
        this.RegisterMessaging();
        Init();
    }
}