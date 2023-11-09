using System;
using System.Runtime.CompilerServices;

namespace ECSDomain.Messages;
public sealed class StructQueue<T> //TODO actually finish this implementation
    where T : struct
{
    private T[] buffer;

    private int head;
    private int tail;
    private int count;

    public StructQueue(in int startingSize = 2)
    {
        buffer = new T[startingSize];
        head = 0;
        tail = 0;
        count = 0;
    }

    public void Enqueue(in T item)
    {
        if (count == buffer.Length) // check if out of space
        {
            var oldSize = buffer.Length;
            var newSize = (int) (oldSize * 1.5) + 1; //TODO perf consider different scaling factor like 1.1
            Array.Resize(ref buffer, newSize);
            var isEmpty = count == 0;

            if (!isEmpty)
            {
                var diff = newSize - oldSize;
                    var len = oldSize - head;

            }
        }

        buffer[head] = item;
        AdvancePointer(ref head);
        count++;
    }

    public bool TryDequeue(out T item)
    {
        var isEmpty = count == 0;
        if (isEmpty)
        {
            item = default;
            return false;
        }

        item = buffer[tail];
        AdvancePointer(ref tail);
        count--;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AdvancePointer(ref int pointer)
    {
        pointer++;
        if (pointer == buffer.Length) // wrap
        {
            pointer = 0;
        }
    }
}