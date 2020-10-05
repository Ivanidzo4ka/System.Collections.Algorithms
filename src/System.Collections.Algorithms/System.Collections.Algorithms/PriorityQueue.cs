namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represent queue in which removing elements would respect it's key.
    /// </summary>
    /// <typeparam name="TKey">Type of key in queue.</typeparam>
    /// <typeparam name="TValue">Type of the value in the queue.</typeparam>
    [DebuggerTypeProxy(typeof(PriorityQueueDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class PriorityQueue<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        private readonly IComparer<TKey> _comparer;
        private readonly List<KeyValuePair<TKey, TValue>> _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{TKey, TValue}"/> class.
        /// Creates default priority queue. Minumum elements are on top of queue.
        /// </summary>
        public PriorityQueue()
            : this(comparer: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{TKey, TValue}"/> class that is empty, has the specified initial capacity, and uses the default <see cref="IComparer{TKey}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="PriorityQueue{TKey, TValue}"/> can contain.</param>
        public PriorityQueue(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{TKey, TValue}"/> class.
        /// Creates priority queue where order defined by provided comparer.
        /// </summary>
        /// <param name="comparer">Comparer to manage order of the keys.</param>
        public PriorityQueue(IComparer<TKey>? comparer)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            _data = new List<KeyValuePair<TKey, TValue>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{TKey, TValue}"/> class that is empty, has the specified initial capacity, and uses the specified <see cref="IComparer{TKey}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="PriorityQueue{TKey, TValue}"/> can contain.</param>
        /// <param name="comparer">Comparer to manage order of the keys.</param>
        public PriorityQueue(int capacity, IComparer<TKey>? comparer)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            _data = new List<KeyValuePair<TKey, TValue>>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{TKey, TValue}"/> class.
        /// Creates priority queue and fill it with provided pairs of key and value.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="PriorityQueue{TKey, TValue}"/>.</param>
        public PriorityQueue(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="PriorityQueue{TKey, TValue}"/>.</param>
        /// <param name="comparer">Comparer to manage order of the keys.</param>
        public PriorityQueue(IEnumerable<KeyValuePair<TKey, TValue>> collection, IComparer<TKey>? comparer)
            : this(comparer)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            _data.AddRange(collection);
            for (int i = _data.Count / 2; i >= 0; i--)
            {
                Heapify(i);
            }
        }

        /// <summary>
        /// Gets the number of elements that are contained in a priority queue.
        /// </summary>
        public int Count => _data.Count;

        /// <summary>
        /// Gets the <see cref="IComparer{TKey}"/> for the priority queue.
        /// </summary>
        public IComparer<TKey> Comparer => _comparer;

        /// <summary>
        /// Gets a value indicating whether prioirty queue is empty or not.
        /// </summary>
        public bool IsEmpty => _data.Count == 0;

        /// <summary>
        /// Adds an object to the into the <see cref="PriorityQueue{TKey,TValue}"/> by its priority.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
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

        /// <summary>
        /// Removes and returns top object according to priority in the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <returns>Top object according to priority that is removed from the <see cref="PriorityQueue{TKey,TValue}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PriorityQueue{TKey,TValue}"/> is empty.</exception>
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

        /// <summary>
        /// Removes and returns top object according to priority in the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <param name="item">When this method returns, the value according to priority, if the queue is non empty; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the <see cref="PriorityQueue{TKey,TValue}"/> is non-empty; otherwise, false.</returns>
        public bool TryDequeue(out KeyValuePair<TKey, TValue> item)
        {
            item = default;
            if (_data.Count == 0)
                return false;

            item = _data[0];
            _data[0] = _data[_data.Count - 1];
            _data.RemoveAt(_data.Count - 1);
            Heapify(0);
            return true;
        }

        /// <summary>
        /// Returns top object according to priority in the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <returns>Object according to priority from the <see cref="PriorityQueue{TKey,TValue}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PriorityQueue{TKey,TValue}"/> is empty.</exception>
        public KeyValuePair<TKey, TValue> Peek()
        {
            if (_data.Count == 0)
                ThrowForEmptyQueue();
            return _data[0];
        }

        /// <summary>
        /// Returns top object according to priority in the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <param name="item">When this method returns, the value according to priority, if the queue is non empty; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the <see cref="PriorityQueue{TKey,TValue}"/> is non-empty; otherwise, <see langword="false"/>.</returns>
        public bool TryPeek(out KeyValuePair<TKey, TValue> item)
        {
            item = default;
            if (_data.Count == 0)
                return false;
            item = _data[0];
            return true;
        }

        /// <summary>
        /// Determines whether the <see cref="PriorityQueue{TKey,TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="PriorityQueue{TKey,TValue}"/>. The value can be null for reference types.</param>
        /// <returns><see langword="true"/> if the <see cref="PriorityQueue{TKey,TValue}"/> contains an element with the specified value; otherwise, <see langword="false"/>.</returns>
        public bool ContainsValue(TValue value)
        {
            foreach (var pair in _data)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the first element with the specified value from the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <param name="value">The element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if key was not found in the original <see cref="PriorityQueue{TKey,TValue}"/>.</returns>
        public bool Remove(TValue value)
        {
            int pos;
            for (pos = 0; pos < _data.Count; pos++)
            {
                if (EqualityComparer<TValue>.Default.Equals(_data[pos].Value, value))
                    break;
            }

            if (pos != _data.Count)
            {
                _data[pos] = _data[_data.Count - 1];
                _data.RemoveAt(_data.Count - 1);
                Heapify(pos);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="PriorityQueue{TKey,TValue}"/>.</returns>
        public IEnumerator GetEnumerator() => ((ICollection)_data).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="PriorityQueue{TKey,TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="PriorityQueue{TKey,TValue}"/>.</returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _data.GetEnumerator();

        /// <summary>
        /// Converts priority queue into <see cref="KeyValuePair{TKey, TValue}"/>[].
        /// </summary>
        /// <returns>Array of elements.</returns>
        internal KeyValuePair<TKey, TValue>[] ToArray() => _data.ToArray();

        private void ThrowForEmptyQueue()
        {
            throw new InvalidOperationException("Queue is empty.");
        }

        private void Heapify(int position)
        {
            int leftChild;
            int rightChild;
            int largestChild;

            while (true)
            {
                leftChild = (2 * position) + 1;
                rightChild = (2 * position) + 2;
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

        /// <summary>
        /// Class change how <see cref="PriorityQueue{TKey, TValue}"/> displayed in debugger view.
        /// </summary>
        /// <typeparam name="TPriority">Type of priority in queue.</typeparam>
        /// <typeparam name="TElement">Type of element in queue.</typeparam>
        internal sealed class PriorityQueueDebugView<TPriority, TElement>
        {
            private readonly PriorityQueue<TPriority, TElement> _queue;

            /// <summary>
            /// Initializes a new instance of the <see cref="PriorityQueueDebugView{TPriority, TElement}"/> class which wraps queue for display it debugger view.
            /// </summary>
            /// <param name="queue">Queue to wrap.</param>
            public PriorityQueueDebugView(PriorityQueue<TPriority, TElement> queue)
            {
                if (queue == null)
                {
                    throw new ArgumentNullException(nameof(queue));
                }

                _queue = queue;
            }

            /// <summary>
            /// Gets array of <see cref="KeyValuePair{TKey, TValue}"/>.
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<TPriority, TElement>[] Items
            {
                get
                {
                    return _queue.ToArray();
                }
            }
        }
    }
}
