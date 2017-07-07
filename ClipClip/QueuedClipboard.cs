using System;
using System.Collections.Generic;

namespace ClipClip
{
    public class QueuedClipboard<T>
    {
        private Queue<T> queue = new Queue<T>();
        private Int32 limit;

        public QueuedClipboard(Int32 limit)
        {
            this.limit = limit;
        }

        public Int32 Limit {
            get {
                return limit;
            }
            set {
                limit = value;
                RemoveItemsOverLimit();
            }
        }

        public Int32 Count {
            get {
                return queue.Count;
            }
        }

        private void RemoveItemsOverLimit()
        {
            if (queue.Count > limit)
            {
                for (Int32 i = 0; i < queue.Count - limit; i++)
                {
                    queue.Dequeue();
                }
            }
        }

        public void Enqueue(T obj)
        {
            queue.Enqueue(obj);
            RemoveItemsOverLimit();
        }

        public T Dequeue()
        {
            return queue.Dequeue();
        }

        public T Peek()
        {
            return queue.Peek();
        }
    }
}
