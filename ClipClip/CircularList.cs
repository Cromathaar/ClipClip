using System;
using System.Collections.Generic;

namespace ClipClip
{
    public class CircularList<T>
    {
        private Byte itemCount;
        private List<T> items;
        private Int32 index;

        public CircularList(Byte itemCount)
        {
            this.itemCount = itemCount;
            items = new List<T>();
            index = 0;
        }

        public void Add(T item)
        {
            items.Insert(0, item);

            if (items.Count > itemCount)
            {
                items.RemoveAt(items.Count - 1);
            }
        }

        public T GetNext()
        {
            index = (index + 1) % items.Count;
            return items[index];
        }

        public T GetCurrent()
        {
            return items[index];
        }

        public Int32 Count {
            get {
                return items.Count;
            }
        }
    }
}
