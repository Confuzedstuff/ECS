using System.Runtime.CompilerServices;

namespace ECSDomain.Messages;
// public readonly partial struct ReaderId : IPvo<int>
// {
// }
//
// public sealed class MultiReaderStructQueue<T>
//     where T : struct
// {
//     private T[] buffer;
//     private int[] readerReceipts;
//
//     private int head;
//     private int tail;
//     private int count;
//
//     public MultiReaderStructQueue(in int startingSize = 1)
//     {
//         buffer = new T[startingSize];
//         readerReceipts = new int[startingSize];
//         head = 0;
//         tail = 0;
//         count = 0;
//     }
//
//     public void Enqueue(in T item)
//     {
//         if (count == buffer.Length) // check if out of space
//         {
//             var oldSize = buffer.Length;
//             var newSize = (int) (oldSize * 1.5) + 1; //TODO perf consider different scaling factor like 1.1
//             Array.Resize(ref buffer, newSize);
//             Array.Resize(ref readerReceipts, newSize);
//
//             var diff = newSize - oldSize;
//
//             for (var i = oldSize - 1; i >= tail; i--)
//             {
//                 var newIndex = i + diff;
//                 buffer[newIndex] = buffer[i];
//                 readerReceipts[newIndex] = readerReceipts[i];
//             }
//
//             for (var i = tail; i < tail + diff; i++)
//             {
//                 buffer[i] = default;
//                 readerReceipts[i] = default;
//             }
//
//             tail += diff;
//         }
//
//         buffer[head] = item;
//         AdvancePointer(ref head);
//         count++;
//     }
//
//     public bool TryDequeue(in ReaderId readerId,out T item)
//     {
//         var isEmpty = count == 0;
//         if (isEmpty)
//         {
//             item = default;
//             return false;
//         }
//
//         item = buffer[tail];
//         AdvancePointer(ref tail);
//         count--;
//         return true;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private void AdvancePointer(ref int pointer)
//     {
//         pointer++;
//         if (pointer == buffer.Length) // wrap
//         {
//             pointer = 0;
//         }
//     }
// }