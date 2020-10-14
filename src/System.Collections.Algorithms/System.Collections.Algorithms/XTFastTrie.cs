using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Algorithms
{
    public class XFastTrie
    {
        private Node _root;
        private const int Dimension = 8;
        public int Count;
        private Leaf? Begin;
        private Leaf? End;
        private Dictionary<byte, InternalNode>[] _prefixes;

        public XFastTrie()
        {
            _root = new InternalNode();
            Count = 0;
            Begin = null;
            End = null;
            _prefixes = new Dictionary<byte, InternalNode>[Dimension - 1];
            for (int i = 0; i < Dimension - 1; i++)
                _prefixes[i] = new Dictionary<byte, InternalNode>();
        }

        public byte Min => Begin != null ? Begin.Value : byte.MaxValue;
        public byte Max => End != null ? End.Value : byte.MinValue;

        private abstract class Node
        {
            public Node? Left;
            public Node? Right;
            public static int i;
        }

        private class InternalNode : Node
        {
        }

        private class Leaf : Node
        {
            public byte Value;
        }

        private Leaf? PredOrSucc(byte x)
        {
            if (_root.Left == null && _root.Right == null)
                return null;
            Node? current = _root;
            int i;
            for (i = 0; i < Dimension; i++)
            {
                var node = ((x >> (Dimension - 1 - i)) & 1U) == 1U ? current.Right : current.Left;
                if (node is Leaf)
                    return node as Leaf;
                if (node is null)
                    return current.Right != null ? (Leaf)current.Right : (Leaf)current.Left;
                current = node;
            }

            return current.Right != null ? (Leaf)current.Right : (Leaf)current.Left;
        }

        private Leaf? PredOrSuccPrefix(byte x)
        {
            if (_root.Left == null && _root.Right == null)
                return null;
            int low = 0, high = Dimension - 1;
            InternalNode found = null;
            while (high - low > 1)
            {
                int mid = (low + high) / 2;
                byte prefix = (byte)(x >> (Dimension - 1 - mid));
                if (_prefixes[mid].TryGetValue(prefix, out InternalNode look))
                {
                    low = mid;
                    found = look;
                }
                else
                    high = mid;
            }
            if (low == Dimension - 2)
            {
                var t = found.Right == null ? (Leaf)found.Left : (Leaf)found.Right;
                if (t.Value != x)
                    return t;
                else
                    return t.Right != null ? (Leaf)t.Right : (Leaf)t.Left;
            }
            if (found.Left is Leaf result)
                return result;
            else
                return found.Right as Leaf;
        }

        public bool TryGetNext(byte x, out byte result)
        {
            var predOrSucc = PredOrSuccPrefix(x);
            var pref = PredOrSuccPrefix(x);
            result = 0;
            if (predOrSucc == null)
                return false;
            if (predOrSucc.Value < x)
            {
                var right = predOrSucc.Right as Leaf;
                if (right != null)
                {
                    if (right.Value > x)
                        result = right.Value;
                    else
                    {
                        if (right.Right is Leaf secondRight)
                            result = secondRight.Value;
                        else
                            return false;
                    }
                }
            }
            else
            {
                result = predOrSucc.Value;
            }

            return true;
        }

        public bool TryGetPrevious(byte x, out byte result)
        {
            var predOrSucc = PredOrSuccPrefix(x);
            //var pref = PredOrSuccPrefix(x);
            result = 0;
            if (predOrSucc == null)
                return false;
            if (predOrSucc.Value > x)
            {
                var left = predOrSucc.Left as Leaf;
                if (left != null)
                {
                    if (left.Value < x)
                        result = left.Value;
                    else
                    {
                        if (left.Left is Leaf secondLeft)
                            result = secondLeft.Value;
                        else
                            return false;
                    }
                }
                else
                    return false;
            }
            else
            {
                result = predOrSucc.Value;
            }

            return true;
        }

        private void RemoveLeaf(Leaf leaf)
        {
            if (leaf.Left != null && leaf.Right != null)
            {
                var left = leaf.Left;
                var right = leaf.Right;
                left.Right = right;
                right.Left = left;
            }
            else
            {
                if (leaf.Left != null)
                {
                    End = (Leaf)leaf.Left;
                    leaf.Left.Right = null;

                }
                else if (leaf.Right != null)
                {
                    Begin = (Leaf)leaf.Right;
                    leaf.Right.Left = null;
                }
                else
                {
                    Begin = null;
                    End = null;
                }
            }

            Count--;
        }

        private void InsertRightLeaf(Leaf leaf, Leaf addition)
        {
            var temp = leaf.Right;
            leaf.Right = addition;
            addition.Left = leaf;
            addition.Right = temp;
            if (temp != null)
                temp.Left = addition;
            if (addition.Value > Max)
                End = addition;
        }

        private void InsertLeftLeaf(Leaf leaf, Leaf addition)
        {
            var temp = leaf.Left;
            leaf.Left = addition;
            addition.Right = leaf;
            addition.Left = temp;
            if (temp != null)
                temp.Right = addition;
            if (addition.Value < Min)
                Begin = addition;
        }

        public bool Add(byte x)
        {
            Node current = _root;
            Stack<(Node, bool)> stack = new Stack<(Node, bool)>();
            Leaf? predOrSucc = null;
            for (int i = 0; i < Dimension - 1; i++)
            {
                byte prefix = (byte)(x >> (Dimension - 1 - i));
                if ((prefix & 1U) == 1U)
                {
                    stack.Push((current, true));
                    if (!(current.Right is InternalNode))
                    {
                        predOrSucc ??= current.Right as Leaf;
                        var newNode = new InternalNode();
                        _prefixes[i].Add(prefix, newNode);
                        current.Right = newNode;
                    }

                    current = current.Right;
                }
                else
                {
                    stack.Push((current, false));
                    if (!(current.Left is InternalNode))
                    {
                        predOrSucc ??= current.Left as Leaf;
                        var newNode = new InternalNode();
                        _prefixes[i].Add(prefix, newNode);
                        current.Left = newNode;
                    }

                    current = current.Left;
                }

            }

            Leaf? added = null;
            if ((x & 1U) == 1U)
            {
                if (current.Right is null)
                {
                    predOrSucc ??= current.Left as Leaf;
                    added = new Leaf() { Value = x };
                    current.Right = added;
                    if (predOrSucc != null)
                    {
                        if (predOrSucc.Value < x)
                            InsertRightLeaf(predOrSucc, added);
                        else
                            InsertLeftLeaf(predOrSucc, added);
                    }
                }
            }
            else
            {
                if (current.Left is null)
                {
                    predOrSucc ??= current.Right as Leaf;
                    added = new Leaf() { Value = x };
                    current.Left = added;
                    if (predOrSucc != null)
                    {
                        if (predOrSucc.Value > x)
                            InsertLeftLeaf(predOrSucc, added);
                        else
                            InsertRightLeaf(predOrSucc, added);
                    }
                }
            }
            if (added is null)
                return false;
            while (stack.Count != 0)
            {
                var (node, direction) = stack.Pop();
                if (direction)
                {
                    if (!(node.Left is InternalNode))
                    {
                        var currentLeaf = node.Left as Leaf;
                        if (currentLeaf == null || currentLeaf.Value > added.Value)
                            node.Left = added;
                    }
                }
                else
                {
                    if (!(node.Right is InternalNode))
                    {
                        var currentLeaf = node.Right as Leaf;
                        if (currentLeaf == null || currentLeaf.Value < added.Value)
                            node.Right = added;
                    }
                }
            }
            Count++;
            return true;
        }

        public bool Find(byte x)
        {
            Node? current = _root;
            for (int i = 0; i < Dimension; i++)
            {
                current = ((x >> (Dimension - 1 - i)) & 1U) == 1U ? current.Right : current.Left;
                if (current is null || (current is Leaf && (i < Dimension - 1))) return false;
            }
            return true;
        }

        public bool Remove(byte x)
        {
            Node? current = _root;
            int i;
            Stack<Node> stack = new Stack<Node>();
            for (i = 0; i < Dimension - 1; i++)
            {
                stack.Push(current);
                current = ((x >> (Dimension - 1 - i)) & 1U) == 1U ? current.Right : current.Left;
                if (current is null || current is Leaf) return false;

            }
            Leaf? removed;
            bool destroy = true;
            if ((x & 1U) == 1U)
            {
                if (current.Right == null)
                    return false;
                RemoveLeaf((Leaf)current.Right);
                removed = (Leaf)current.Right;
                current.Right = null;
                if (current.Left != null)
                    destroy = false;
            }
            else
            {
                if (current.Left == null)
                    return false;
                RemoveLeaf((Leaf)current.Left);
                removed = (Leaf)current.Left;
                current.Left = null;
                if (current.Right != null)
                    destroy = false;
            }

            while (stack.Count != 0)
            {
                var node = stack.Pop();
                byte prefix = (byte)(x >> (Dimension - 1 - stack.Count));
                if (((x >> (Dimension - 1 - stack.Count)) & 1U) == 1U)
                {
                    if (destroy)
                    {
                        _prefixes[stack.Count].Remove(prefix);
                        node.Right = null;
                    }
                    var leaf = node.Left as Leaf;
                    if (leaf != null && leaf.Value == removed.Value)
                        node.Left = leaf.Right;
                }
                else
                {
                    if (destroy)
                    {
                        _prefixes[stack.Count].Remove(prefix);
                        node.Left = null;
                    }
                    var leaf = node.Right as Leaf;
                    if (leaf != null && leaf.Value == removed.Value)
                        node.Right = leaf.Left;
                }

                if (destroy && (node.Left is InternalNode || node.Right is InternalNode)) destroy = false;
                i--;
            }
            return true;
        }
    }
}
