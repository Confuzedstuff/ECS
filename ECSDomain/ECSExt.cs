using ECSDomain.Messages;

namespace ECSDomain;
public static class ECSExt
{
    public static void InjectArchetypes(this ECS ecs, object target)
    {
        var currentType = target.GetType();
        var fields = currentType.GetAllFields();
        var archFields = fields.Where(x => typeof(Archetype).IsAssignableFrom(x.FieldType));
        var getArchMethod = typeof(ECS).GetMethod(nameof(ECS.GetArchetype));
        foreach (var fieldInfo in archFields)
        {
            var fungen = getArchMethod.MakeGenericMethod(fieldInfo.FieldType);
            var instance = fungen.Invoke(ecs, Array.Empty<object>());
            fieldInfo.SetValue(target, instance);
        }
    }

    public static void InjectMessaging(this ECS ecs, object system)
    {
        RegisterReaders(ecs, system);
        RegisterWriters(ecs, system);
    }

    private static void RegisterWriters(ECS ecs, object system) => Register<MessageBuffer>(ecs, system, nameof(ECS.GetMessageWriter));
    private static void RegisterReaders(ECS ecs, object system) => Register<MessageReader>(ecs, system, nameof(ECS.RegisterMessageReader));

    private static void Register<T>(ECS ecs, object system, string method)
    {
        var currentType = system.GetType();
        var fields = currentType.GetAllFields();
        var readers = fields.Where(x => typeof(T).IsAssignableFrom(x.FieldType));
        var fun = typeof(ECS).GetMethod(method);
        foreach (var fieldInfo in readers)
        {
            var genericType = fieldInfo.FieldType.GenericTypeArguments.First();
            var fungen = fun.MakeGenericMethod(genericType);
            var instance = fungen.Invoke(ecs, Array.Empty<object>());
            fieldInfo.SetValue(system, instance);
        }
    }
}