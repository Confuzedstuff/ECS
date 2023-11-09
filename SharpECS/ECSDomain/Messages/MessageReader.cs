using System.Collections.Generic;

namespace ECSDomain.Messages;

public abstract class MessageReader
{
    
}
public class MessageReader<T> : MessageReader
    where T : struct
{
    //private readonly StructQueue<T> queue = new();
    private readonly Queue<T> queue = new();//TODO perf replace with struct queue

    public void Enqueue(in T message) => queue.Enqueue(message);
    public bool TryDequeue(out T message) => queue.TryDequeue(out message);
}