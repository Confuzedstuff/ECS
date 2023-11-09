using System.Reflection;

namespace ECSDomain;
public static class ReflectionExt
{
    public static IEnumerable<Type> GetTypesThatInheritFrom<T>(this Type[] types)
    {
        return types
            .Where(t => typeof(T).IsAssignableFrom(t))
            .Where(t => t != typeof(T));
    }

    public static HashSet<FieldInfo> GetAllFields(this Type type)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToHashSet();
        while (type.BaseType != null)
        {
            type = type.BaseType;
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                fields.Add(fieldInfo);
            }
        }

        return fields;
    }
}