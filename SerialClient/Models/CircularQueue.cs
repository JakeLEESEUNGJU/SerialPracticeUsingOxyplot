using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialClient
{
    public class CircularQueue<T> where T : struct
    {
        private readonly T[] queue;
        private int readPosition = 0;
        private int writePosition = 0;

        public int Count
        {
            get
            {
                var count = writePosition - readPosition;
                return count > queue.Length ? queue.Length : count;
            }
        }


        public CircularQueue(int capacity=32768)
        {
            queue = new T[capacity];
            readPosition = 0;
        }

        public T[] Peek(int desireCount)
        {
            var actualCount = Math.Min(Count, desireCount);
            T[] buf = new T[actualCount];
            var actualReadPosition = readPosition % queue.Length;
            var readableCount = actualReadPosition + actualCount > queue.Length ? queue.Length - actualReadPosition : actualCount;
            var remainCount = actualCount - readableCount;

            Buffer.BlockCopy(queue, actualReadPosition, buf, 0, readableCount);
            if (remainCount != 0) Buffer.BlockCopy(queue, 0, buf, readableCount, remainCount);
            return buf;
        }

        public T[] Get(int desireCount)
        {
            var actualCount = Math.Min(Count, desireCount);
            T[] buf = new T[actualCount];
            var actualReadPosition = readPosition % queue.Length;
            var readableCount = actualReadPosition + actualCount > queue.Length ? queue.Length - actualReadPosition : actualCount;
            var remainCount = actualCount - readableCount;
            Buffer.BlockCopy(queue, actualReadPosition, buf, 0, readableCount);
            if (remainCount != 0) Buffer.BlockCopy(queue, 0, buf, readableCount, remainCount);
            readPosition += actualCount;
            return buf;
        }

        public void Write(T[] buf)
        {
            Buffer.BlockCopy(buf, 0, queue, writePosition % queue.Length, buf.Length);
            writePosition += buf.Length;
        }
    }
}