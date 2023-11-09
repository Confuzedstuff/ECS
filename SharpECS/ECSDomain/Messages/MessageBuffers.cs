using System;
using System.Collections.Generic;

namespace ECSDomain.Messages;
public class MessageBuffers
{
    private readonly Dictionary<Type, object> buffers = new();

    public MessageBuffer<T> GetBuffer<T>() where T : struct
    {
        if (!buffers.TryGetValue(typeof(T), out var buffer))
        {
            buffer = new MessageBuffer<T>();
            buffers.Add(typeof(T), buffer);
        }

        return buffer as MessageBuffer<T>;
    }

    public MessageReader<T> RegisterReader<T>() where T : struct
    {
        var buffer = GetBuffer<T>();
        return buffer.RegisterReader();
    }
}