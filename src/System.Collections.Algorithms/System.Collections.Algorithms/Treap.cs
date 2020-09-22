using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Algorithms
{
    public class Treap<T> : IEnumerable, IEnumerable<T>
    {
        private readonly IComparer<T> _comparer;

        private readonly Random _random;
        private Node? _root;
        private int _version;

        public Treap()
            : this((IComparer<T>?)null)
        {
        }

        public Treap(IComparer<T>? comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _random = new Random();
            _version = 0;
        }

        public Treap(IEnumerable<T> collection)
            : this((IComparer<T>?)null)
        {
            T[] elements = collection.ToArray();
            int count = elements.Length;
            if (count > 0)
            {
                Array.Sort(elements, 0, count, _comparer);
                _root = Build(elements, 0, count);
            }
        }

        public int Count => GetCount(_root);

        public void Add(T item, int? prior = null)
        {
            var toAdd = new Node(item, prior ?? _random.Next());
            _version++;
            Insert(ref _root, toAdd);
        }

        public bool Remove(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
            if (_root is null)
                return false;
            _version++;
            return Erase(ref _root, item);
        }

        public void Clear()
        {
            _root = null;
            _version++;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int index]
        {
            get
            {
                if (index < 0 || GetCount(_root) < index)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return GetKthElement(index);
            }
        }

        public IEnumerable<T> Reverse()
        {
            Enumerator e = new Enumerator(this, reverse: true);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

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

        private void Split(Node? current, T key, ref Node? L, ref Node? R)
        {
            if (current is null)
            {
                L = null;
                R = null;
            }
            else
                if (_comparer.Compare(key, current.Value) == -1)
            {
                Split(current.Left, key, ref L, ref current.Left);
                R = current;
            }
            else
            {
                Split(current.Right, key, ref current.Right, ref R);
                L = current;
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
                var leftSize = GetCount(current.Left);
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
                    current = current.Left;
            }

            return result;
        }

        internal class Node
        {
            public readonly T Value;
            public int Prior;
            public int Size;
            public Node? Left;
            public Node? Right;

            public Node(T value, int prior, Node? left = null, Node? right = null)
            {
                Value = value;
                Prior = prior;
                Left = left;
                Right = right;
            }
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly Treap<T> _treap;
            private readonly int _version;

            private readonly Stack<Node> _stack;
            private Node? _current;

            private readonly bool _reverse;

            internal Enumerator(Treap<T> treap)
                : this(treap, reverse: false)
            {
            }

            internal Enumerator(Treap<T> treap, bool reverse)
            {
                _treap = treap;
                _version = treap._version;

                // 2 log(n + 1) is the maximum height.
                _stack = new Stack<Node>((2 * Log2(treap.Count)) + 1);
                _current = null;
                _reverse = reverse;

                Initialize();
            }

            private static int Log2(int value)
            {
                int result = 0;
                while (value > 0)
                {
                    result++;
                    value >>= 1;
                }

                return result;
            }

            private void Initialize()
            {
                _current = null;
                Node? node = _treap._root;
                Node? next;
                while (node != null)
                {
                    next = (_reverse ? node.Right : node.Left);
                    _stack.Push(node);
                    node = next;
                }
            }

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

            public void Dispose() { }

            public T Current
            {
                get
                {
                    if (_current != null)
                    {
                        return _current.Value;
                    }
                    return default(T)!; // Should only happen when accessing Current is undefined behavior
                }
            }

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

            internal void Reset()
            {
                if (_version != _treap._version)
                {
                    throw new InvalidOperationException("Treap changed during enumeration.");
                }

                _stack.Clear();
                Initialize();
            }

            void IEnumerator.Reset() => Reset();
        }
    }
}
