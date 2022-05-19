using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace waPlanner.BackgroundQueue
{
    public interface ISendDocumentsQueue<T>
    {
        void Enqueue(T item);

        T Dequeue();

        IEnumerable<T> GetAll();

        T[] ToArray();
    }

    public class SendDocumentsQueue<T> : ISendDocumentsQueue<T> where T : class
    {
        private readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();

        public void Enqueue(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _items.Enqueue(item);
        }

        public T Dequeue()
        {
            var success = _items.TryDequeue(out var workItem);

            return success
                ? workItem
                : null;
        }

        public IEnumerable<T> GetAll()
        {
            T item;

            while (_items.TryDequeue(out item))
                yield return item;
        }

        public T[] ToArray()
        {
            return _items.ToArray();
        }
    }
}