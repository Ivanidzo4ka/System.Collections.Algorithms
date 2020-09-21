using System.Collections.Generic;

namespace System.Collections.Algorithms
{
    public class Treap<T>
    {
        private readonly Comparer<T> _comparer;

        private readonly Random _random;
        private Node? _root;

        public Treap()
        {
            _comparer = Comparer<T>.Default;
            _random = new Random();
        }

        public void Add(T item)
        {
            var toAdd = new Node(item, _random.Next());
            Insert(ref _root, toAdd);
        }

        public bool Remove(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
            if (_root is null)
                return false;
            return Erase(ref _root, item);
        }

        //public static Treap<T> Combine(Treap<T> left, Treap<T> right)
        //{
        //    Unite(left._root, right._root);
        //}

        private void Merge(ref Node? current, Node? left, Node? right)
        {
            if (left == null || right == null)
            {
                current = left == null ? right : left;
            }
            else if (left.Value > right.Value)
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
                current.Size = Count(current.Left) + Count(current.Right) + 1;
        }

        private int Count(Node? current)
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
                if (_comparer.Compare(key, current.Key) == -1)
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
            else if (addition.Value > current.Value)
            {
                Split(current, addition.Key, ref addition.Left, ref addition.Right);
                current = addition;
            }
            else
            {
                if (_comparer.Compare(addition.Key, current.Key) == -1)
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
            var result = _comparer.Compare(current.Key, key);
            if (result == 0)
            {
                Merge(ref current, current.Left, current.Right);
                return true;
            }
            else
            {
                var success = false;
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
            if (left.Value < right.Value)
            {
                var temp = left;
                left = right;
                right = temp;
            }
            Node? tempLeft = null;
            Node? tempRight = null;
            Split(right, left.Key, ref tempLeft, ref tempRight);
            left.Left = Unite(left.Left, tempLeft);
            left.Right = Unite(left.Right, tempRight);
            return left;
        }

        internal class Node
        {
            public readonly T Key;
            public readonly int Value;
            public int Size;
            public Node? Left;
            public Node? Right;

            public Node(T key, int value, Node? left = null, Node? right = null)
            {
                Key = key;
                Value = value;
                Left = left;
                Right = right;
            }

        }
    }
}
