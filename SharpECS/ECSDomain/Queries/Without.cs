using System.Collections.Generic;

namespace ECSDomain;
public class Without<T> : IQueryElement
    where T: struct

{
    public void Evaluate(List<Archetype> items)
    {
        var toRemove = new List<Archetype>();
        foreach (var item in items)
        {
            if (item.HasComponent<T>())
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