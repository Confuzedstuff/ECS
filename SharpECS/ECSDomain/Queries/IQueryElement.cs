using System.Collections.Generic;

namespace ECSDomain;
public interface IQueryElement
{
    void Evaluate(List<Archetype> items);
}