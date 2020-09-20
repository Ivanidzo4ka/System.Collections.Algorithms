using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.Algorithms
{
    [Serializable]
    [DebuggerTypeProxy(typeof(PriorityQueueDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class PriorityQueue<TKey, TValue> : ICollection, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Comparer<TKey> _comparer;
        private readonly List<KeyValuePair<TKey, TValue>> _data;

        public int Count => _data.Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)_data).SyncRoot;


        public PriorityQueue() : this((IComparer<TKey>)null)
        {
        }

        public PriorityQueue(IComparer<TKey>? comparer)
        {
            _comparer = new KeyComparer(comparer);
            _data = new List<KeyValuePair<TKey, TValue>>();
        }

        public PriorityQueue(IEnumerable<KeyValuePair<TKey, TValue>> priorityQueue) : this(priorityQueue, null)
        {
        }

        public PriorityQueue(IEnumerable<KeyValuePair<TKey, TValue>> priorityQueue, IComparer<TKey> comparer)
        {
            if (priorityQueue is null)
                throw new ArgumentNullException(nameof(priorityQueue));
            _comparer = new KeyComparer(comparer);

            _data = new List<KeyValuePair<TKey, TValue>>();
            _data.AddRange(priorityQueue);
            for (int i = _data.Count / 2; i >= 0; i--)
            {
                Heapify(i);
            }
        }

        private void Heapify(int position)
        {
            int leftChild;
            int rightChild;
            int largestChild;

            while (true)
            {
                leftChild = 2 * position + 1;
                rightChild = 2 * position + 2;
                largestChild = position;

                if (leftChild < _data.Count && _comparer.Compare(_data[leftChild].Key, _data[largestChild].Key) == -1)
                    largestChild = leftChild;

                if (rightChild < _data.Count && _comparer.Compare(_data[rightChild].Key, _data[largestChild].Key) == -1)
                    largestChild = rightChild;

                if (largestChild == position)
                    break;

                var temp = _data[position];
                _data[position] = _data[largestChild];
                _data[largestChild] = temp;
                position = largestChild;
            }
        }

        private void ThrowForEmptyQueue()
        {
            throw new InvalidOperationException("Queue is empty.");
        }

        public void Enqueue(TKey key, TValue value)
        {
            _data.Add(new KeyValuePair<TKey, TValue>(key, value));
            var i = _data.Count - 1;
            var parent = (i - 1) / 2;
            while (i > 0 && _comparer.Compare(_data[parent].Key, _data[i].Key) == 1)
            {
                var temp = _data[i];
                _data[i] = _data[parent];
                _data[parent] = temp;

                i = parent;
                parent = (i - 1) / 2;
            }
        }

        public KeyValuePair<TKey, TValue> Dequeue()
        {
            if (_data.Count == 0)
                ThrowForEmptyQueue();
            var result = _data[0];
            _data[0] = _data[_data.Count - 1];
            _data.RemoveAt(_data.Count - 1);
            Heapify(0);
            return result;
        }

        public KeyValuePair<TKey, TValue> Peek()
        {
            if (_data.Count == 0)
                ThrowForEmptyQueue();
            return _data[0];
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            _data.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_data).CopyTo(array, index);
        }
        public KeyValuePair<TKey, TValue>[] ToArray() => _data.ToArray();

        public IEnumerator GetEnumerator() => ((ICollection)_data).GetEnumerator();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _data.GetEnumerator();

        [Serializable]
        public sealed class KeyComparer : Comparer<TKey>
        {
            // Do not rename (binary serialization)
            internal IComparer<TKey> keyComparer;


            public KeyComparer(IComparer<TKey>? keyComparer)
            {
                if (keyComparer == null)
                {
                    this.keyComparer = Comparer<TKey>.Default;
                }
                else
                {
                    this.keyComparer = keyComparer;
                }
            }

            public override int Compare(TKey x, TKey y)
            {
                return keyComparer.Compare(x, y);
            }
        }

        internal sealed class PriorityQueueDebugView<T, V>
        {
            private readonly PriorityQueue<T, V> _queue;

            public PriorityQueueDebugView(PriorityQueue<T, V> queue)
            {
                if (queue == null)
                {
                    throw new ArgumentNullException(nameof(queue));
                }

                _queue = queue;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<T, V>[] Items
            {
                get
                {
                    return _queue.ToArray();
                }
            }
        }
    }
}
