using System.Collections.Generic;

namespace ECSDomain.Messages;

public class MessageBuffer
{
    
}
public class MessageBuffer<T> : MessageBuffer
    where T : struct
{
    private readonly List<MessageReader<T>> readers = new();

    public void Send(in T message)
    {
        foreach (var reader in readers)
        {
            reader.Enqueue(message);
        }
    }

    public MessageReader<T> RegisterReader()
    {
        var reader = new MessageReader<T>();
        readers.Add(reader);
        return reader;
    }
}