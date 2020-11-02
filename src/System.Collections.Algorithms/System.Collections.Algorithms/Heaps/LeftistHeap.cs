namespace System.Collections.Algorithms.Heaps
{
    using System.Collections.Generic;

    public class LeftistHeap<TKey, TValue>
    {
        private Node<TKey, TValue> _root;

        public LeftistHeap(IEnumerable<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer)
        {
            Comparer = comparer ?? Comparer<TKey>.Default;

            Queue<Node<TKey, TValue>> queue = new Queue<Node<TKey, TValue>>();
            foreach (var elem in data)
            {
                queue.Enqueue(new Node<TKey, TValue>() { Key = elem.Key, Value = elem.Value, Rank = 1 });
            }

            while (queue.Count > 1)
            {
                var x = queue.Dequeue();
                var y = queue.Dequeue();
                queue.Enqueue(MergeInternal(x, y));
            }

            if (queue.Count == 1)
            {
                _root = queue.Dequeue();
            }

        }

        public IComparer<TKey> Comparer { get; }

        public int Count { get; private set; }

        public bool IsEmpty => _root == null;

        public void Merge(LeftistHeap<TKey, TValue> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            _root = MergeInternal(_root, other._root);
        }

        /// <summary>
        /// Adds an object to the into the <see cref="LeftistHeap{TKey, TValue}"/> by its priority.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Enqueue(TKey key, TValue value)
        {
            var toAdd = new Node<TKey, TValue>() { Key = key, Value = value, Rank = 1 };
            _root = MergeInternal(_root, toAdd);
            Count++;
        }

        /// <summary>
        /// Returns top object according to priority in the <see cref="LeftistHeap{TKey,TValue}"/>.
        /// </summary>
        /// <returns>Object according to priority from the <see cref="LeftistHeap{TKey,TValue}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="LeftistHeap{TKey,TValue}"/> is empty.</exception>
        public Node<TKey, TValue> Peek()
        {
            if (_root is null)
                throw ThrowForEmptyQueue();
            return _root;
        }

        /// <summary>
        /// Returns top object according to priority in the <see cref="LeftistHeap{TKey,TValue}"/>.
        /// </summary>
        /// <param name="item">When this method returns, the value according to priority, if the queue is non empty; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the <see cref="LeftistHeap{TKey,TValue}"/> is non-empty; otherwise, <see langword="false"/>.</returns>
        public bool TryPeek(out Node<TKey, TValue>? item)
        {
            item = default;
            if (_root is null)
                return false;
            item = _root;
            return true;
        }

        /// <summary>
        /// Removes and returns top object according to priority in the <see cref="LeftistHeap{TKey,TValue}"/>.
        /// </summary>
        /// <returns>Top object according to priority that is removed from the <see cref="LeftistHeap{TKey,TValue}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="LeftistHeap{TKey,TValue}"/> is empty.</exception>
        public Node<TKey, TValue> Dequeue()
        {
            if (_root is null)
                throw ThrowForEmptyQueue();
            var result = _root;
            _root = MergeInternal(_root.Left, _root.Right);
            result.Left = null;
            result.Right = null;
            result.Rank = 0;
            Count--;
            return result;
        }

        /// <summary>
        /// Removes and returns top object according to priority in the <see cref="LeftistHeap{TKey,TValue}"/>.
        /// </summary>
        /// <param name="item">When this method returns, the value according to priority, if the queue is non empty; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the <see cref="LeftistHeap{TKey,TValue}"/> is non-empty; otherwise, false.</returns>
        public bool TryDequeue(out Node<TKey, TValue>? item)
        {
            item = default;
            if (_root is null)
                return false;

            var result = _root;
            _root = MergeInternal(_root.Left, _root.Right);
            result.Left = null;
            result.Right = null;
            result.Rank = 0;
            Count--;
            return true;
        }

        public class Node<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;
            internal Node<TKey, TValue> Left;
            internal Node<TKey, TValue> Right;
            internal short Rank;
        }

        private Node<TKey, TValue> MergeInternal(Node<TKey, TValue> x, Node<TKey, TValue> y)
        {
            if (x is null) return y;
            if (y is null) return x;

            if (Comparer.Compare(x.Key, y.Key) == 1)
            {
                var temp = x;
                x = y;
                y = temp;
            }

            x.Right = MergeInternal(x.Right, y);
            if (x.Left is null)
            {
                var temp = x.Left;
                x.Left = x.Right;
                x.Right = temp;
                x.Rank = 1;
                return x;
            }

            if (x.Right.Rank > x.Left.Rank)
            {
                var temp = x.Left;
                x.Left = x.Right;
                x.Right = temp;
            }

            x.Rank = (short)(x.Right.Rank + 1);
            return x;

        }

        private Exception ThrowForEmptyQueue() =>
             new InvalidOperationException($"{nameof(LeftistHeap<TKey, TValue>)} is empty.");
    }
}
