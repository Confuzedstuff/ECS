﻿using System;
using System.Collections.Generic;

namespace ECSDomain;

public class With
{
    public static IQueryElement Create(Type type)
    {
        var withType = typeof(With<>).MakeGenericType(type);
        var cons = withType.GetConstructor(Array.Empty<Type>());
        return cons.Invoke(new object[]{}) as IQueryElement;
    }
}

public class With<T> : IQueryElement
where T: struct
{
    public void Evaluate(List<Archetype> items)
    {
        var toRemove = new List<Archetype>();
        foreach (var item in items)
        {
            if (!item.HasComponent<T>())
            {
                toRemove.Add(item);
            }
        }

        foreach (var remove in toRemove)
        {
            items.Remove(remove);
        }
    }
}