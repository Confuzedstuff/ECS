namespace ECSDomain;
public static class ArchetypeExt{
    public static void InjectArchetypes(this object o)
    {
        var currentType = o.GetType();
        var fields = currentType.GetAllFields();
        var archFields = fields.Where(x => typeof(Archetype).IsAssignableFrom(x.FieldType));
        var getArchMethod = ECS.Instance.GetType().GetMethod(nameof(ECS.GetArchetype));
        foreach (var fieldInfo in archFields)
        {
            var fungen = getArchMethod.MakeGenericMethod(fieldInfo.FieldType);
            var instance = fungen.Invoke(ECS.Instance, Array.Empty<object>());
            fieldInfo.SetValue(o, instance);
        }
    }
}