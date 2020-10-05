namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Treap, aka Cartesian Tree, is randomized balanced search tree.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class Treap<T> : IEnumerable, IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly IComparer<T> _comparer;
        private readonly Random _random;
        private Node? _root;
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="Treap{T}"/> class.
        /// </summary>
        public Treap()
            : this((IComparer<T>?)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Treap{T}"/> class that is empty and is sorted according to the specified <see cref="IComparer{T}"/> interface.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements.</param>
        public Treap(IComparer<T>? comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _random = new Random();
            _version = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Treap{T}"/> class that contains elements copied from a specified enumerable collection..
        /// </summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        public Treap(IEnumerable<T> collection)
            : this(collection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Treap{T}"/>  class that contains elements copied from a specified enumerable collection and that uses a specified comparer.
        /// </summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <param name="comparer">The default comparer to use for comparing objects.</param>
        public Treap(IEnumerable<T> collection, IComparer<T>? comparer)
            : this(comparer)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            T[] elements = collection.ToArray();
            int count = elements.Length;
            if (count > 0)
            {
                Array.Sort(elements, 0, count, _comparer);
                _root = Build(elements, 0, count);
            }
        }

        /// <summary>
        /// Gets the number of elements in the <see cref="Treap{T}"/>.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => GetCount(_root);

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || GetCount(_root) <= index)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return GetKthElement(index);
            }
        }

        /// <summary>
        /// Adds an element to the treap.
        /// </summary>
        /// <remarks>
        /// Adding element is O(logN) operation.
        /// The <see cref="Treap{T}"/> class does accept duplicate elements.
        /// </remarks>
        /// <param name="item">The element to add to the set.</param>
        public void Add(T item)
        {
            var toAdd = new Node(item, _random.Next());
            _version++;
            Insert(ref _root, toAdd);
        }

        /// <summary>
        /// Removes a specified item from the <see cref="Treap{T}"/>.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><see langword="true"/> if the element is found and successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// If the <see cref="Treap{T} "/> object does not contain the specified element, the object remains unchanged and no exception is thrown.
        /// item can be null for reference types.
        /// This method is an O(log n) operation.
        /// </remarks>
        public bool Remove(T item)
        {
            if (_root is null)
                return false;
            return Erase(ref _root, item);
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        /// <remarks> This method is an O(1) operation.</remarks>
        public void Clear()
        {
            _root = null;
            _version++;
        }

        /// <summary>
        /// Merge other <see cref="Treap{T} "/> into current one and clear other treap.
        /// </summary>
        /// <param name="other">Treap to be merged in.</param>
        /// <remarks> This method is O(Mlog(N/M)) operation.</remarks>
        public void MergeIn(Treap<T> other)
        {
            if (other.Count == 0)
                return;
            _version++;
            _root = Unite(_root, other._root);
            other.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Treap{T}"/>.
        /// </summary>
        /// <returns>An enumerator that iterates through the <see cref="Treap{T}"/> in sorted order.</returns>
        /// <remarks>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="Treap{T}.Enumerator.MoveNext"/> or <see cref="IEnumerator.Reset"/> throws an <see cref="InvalidOperationException"/>.
        /// This method is an O(log n) operation.
        /// </remarks>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that iterates over the <see cref="Treap{T}"/> in reverse order.
        /// </summary>
        /// <returns>An enumerator that iterates over the <see cref="Treap{T}"/> in reverse order.</returns>
        public IEnumerable<T> Reverse()
        {
            Enumerator e = new Enumerator(this, reverse: true);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="Treap{T}"/> contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="Treap{T}"/>.</param>
        /// <returns><see langword="true"/> if the set contains <paramref name="item"/>; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T item)
        {
            if (_root is null)
                return false;
            var current = _root;
            while (current != null)
            {
                var comparison = _comparer.Compare(current.Value, item);
                if (comparison == 0)
                    return true;
                else if (comparison == 1)
                    current = current.Left;
                else
                    current = current.Right;
            }

            return false;
        }

        private void Merge(ref Node? current, Node? left, Node? right)
        {
            if (left == null || right == null)
            {
                current = left == null ? right : left;
            }
            else if (left.Prior > right.Prior)
            {
                Merge(ref left.Right, left.Right, right);
                current = left;
            }
            else
            {
                Merge(ref right.Left, left, right.Left);
                current = right;
            }

            UpdateCount(current);
        }

        private void UpdateCount(Node? current)
        {
            if (current != null)
                current.Size = GetCount(current.Left) + GetCount(current.Right) + 1;
        }

        private int GetCount(Node? current)
        {
            if (current is null)
                return 0;
            return current.Size;
        }

        private void Split(Node? current, T key, ref Node? left, ref Node? rigth)
        {
            if (current is null)
            {
                left = null;
                rigth = null;
            }
            else if (_comparer.Compare(key, current.Value) == -1)
            {
                Split(current.Left, key, ref left, ref current.Left);
                rigth = current;
            }
            else
            {
                Split(current.Right, key, ref current.Right, ref rigth);
                left = current;
            }

            UpdateCount(current);
        }

        private void Insert(ref Node? current, Node addition)
        {
            if (current is null)
            {
                current = addition;
            }
            else if (addition.Prior > current.Prior)
            {
                Split(current, addition.Value, ref addition.Left, ref addition.Right);
                current = addition;
            }
            else
            {
                if (_comparer.Compare(addition.Value, current.Value) == -1)
                    Insert(ref current.Left, addition);
                else
                    Insert(ref current.Right, addition);
            }

            UpdateCount(current);
        }

        private bool Erase(ref Node? current, T key)
        {
            if (current is null)
                return false;
            var result = _comparer.Compare(current.Value, key);
            if (result == 0)
            {
                _version++;
                Merge(ref current, current.Left, current.Right);
                return true;
            }
            else
            {
                bool success;
                if (result == 1)
                    success = Erase(ref current.Left, key);
                else
                    success = Erase(ref current.Right, key);
                if (success)
                    UpdateCount(current);
                return success;
            }
        }

        private Node? Unite(Node? left, Node? right)
        {
            if (left == null || right == null)
            {
                return left == null ? right : left;
            }

            if (left.Prior < right.Prior)
            {
                var temp = left;
                left = right;
                right = temp;
            }

            Node? tempLeft = null;
            Node? tempRight = null;
            Split(right, left.Value, ref tempLeft, ref tempRight);
            left.Left = Unite(left.Left, tempLeft);
            left.Right = Unite(left.Right, tempRight);
            UpdateCount(left);
            return left;
        }

        private void Heapify(Node node)
        {
            while (node != null)
            {
                Node max = node;
                if (node.Left != null && node.Left.Prior > node.Prior)
                    max = node.Left;
                if (node.Right != null && node.Right.Prior > max.Prior)
                    max = node.Right;
                if (max != node)
                {
                    int temp = max.Prior;
                    max.Prior = node.Prior;
                    node.Prior = temp;
                }
                else
                {
                    return;
                }
            }
        }

        private Node? Build(T[] items, int start, int end)
        {
            if (end - start == 0)
                return null;
            int mid = start + ((end - start) / 2);
            Node toAdd = new Node(items[mid], _random.Next());
            toAdd.Left = Build(items, start, mid);
            toAdd.Right = Build(items, mid + 1, end);
            UpdateCount(toAdd);
            Heapify(toAdd);
            return toAdd;
        }

        private T GetKthElement(int k)
        {
            var current = _root;
            if (current is null)
                throw new NotSupportedException("We shouldn't call this function on empty treap.");
            T result;
            while (true)
            {
                var leftSize = GetCount(current!.Left);
                if (leftSize == k)
                {
                    result = current.Value;
                    break;
                }

                if (leftSize < k)
                {
                    k -= leftSize + 1;
                    current = current.Right;
                }
                else
                {
                    current = current.Left;
                }
            }

            return result;
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="Treap{T}"/> object.
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly Treap<T> _treap;
            private readonly int _version;

            private readonly Stack<Node> _stack;

            private readonly bool _reverse;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="treap">Treap to iterate.</param>
            /// <param name="reverse">Should be in reverse order or not.</param>
            internal Enumerator(Treap<T> treap, bool reverse = false)
            {
                _treap = treap;
                _version = treap._version;

                // log(n) is the average height of heap.
                _stack = new Stack<Node>(Utils.Log2(treap.Count));
                _current = null;
                _reverse = reverse;

                Initialize();
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public T Current
            {
                get
                {
                    if (_current != null)
                    {
                        return _current.Value;
                    }

                    return default!; // Should only happen when accessing Current is undefined behavior
                }
            }

            /// <inheritdoc/>
            object? IEnumerator.Current
            {
                get
                {
                    if (_current == null)
                    {
                        throw new InvalidOperationException("Something went terrible wrong.");
                    }

                    return _current.Value;
                }
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                // Make sure that the underlying subset has not been changed since
                if (_version != _treap._version)
                {
                    throw new InvalidOperationException("Treap changed during enumeration.");
                }

                if (_stack.Count == 0)
                {
                    _current = null;
                    return false;
                }

                _current = _stack.Pop();
                Node? node = _reverse ? _current.Left : _current.Right;
                Node? next;
                while (node != null)
                {
                    next = _reverse ? node.Right : node.Left;
                    _stack.Push(node);
                    node = next;
                }

                return true;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
            }

            /// <inheritdoc/>
            void IEnumerator.Reset() => Reset();

            private void Reset()
            {
                if (_version != _treap._version)
                {
                    throw new InvalidOperationException("Treap changed during enumeration.");
                }

                _stack.Clear();
                Initialize();
            }

            private void Initialize()
            {
                _current = null;
                Node? node = _treap._root;
                Node? next;
                while (node != null)
                {
                    next = _reverse ? node.Right : node.Left;
                    _stack.Push(node);
                    node = next;
                }
            }
        }

        private class Node
        {
#pragma warning disable SA1401 // Fields should be private
            public Node? Left;
            public Node? Right;
#pragma warning restore SA1401 // Fields should be private

            public Node(T value, int prior, Node? left = null, Node? right = null)
            {
                Value = value;
                Prior = prior;
                Left = left;
                Right = right;
                Size = 0;
            }

            public T Value { get; private set; }

            public int Prior { get; set; }

            public int Size { get; set; }
        }
    }
}
