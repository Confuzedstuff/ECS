using System.Collections.Generic;

namespace ECSDomain;
public interface IArchQueryElement
{
    void Evaluate(List<Archetype> items);
}