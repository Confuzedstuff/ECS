using System;
using System.Linq;
using ECSDomain.Messages;

namespace ECSDomain;
public static class MessagingExt
{
    public static void InjectMessaging(this object system)
    {
        RegisterReaders(system);
        RegisterWriters(system);
    }

    private static void RegisterWriters(this object system) => Register<MessageBuffer>(system,nameof(ECS.GetMessageWriter));
    private static void RegisterReaders(this object system) => Register<MessageReader>(system, nameof(ECS.RegisterMessageReader));

    private static void Register<T>(this object system, string method)
    {
        var currentType = system.GetType();
        var fields = currentType.GetAllFields();
        var readers = fields.Where(x => typeof(T).IsAssignableFrom(x.FieldType));
        var fun = ECS.Instance.GetType().GetMethod(method);
        foreach (var fieldInfo in readers)
        {
            var genericType = fieldInfo.FieldType.GenericTypeArguments.First();
            var fungen = fun.MakeGenericMethod(genericType);
            var instance = fungen.Invoke(ECS.Instance, Array.Empty<object>());
            fieldInfo.SetValue(system, instance);
        }
    }
}
