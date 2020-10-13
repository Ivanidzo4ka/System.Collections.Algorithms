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
        public XFastTrie()
        {
            _root = new InternalNode();
            Count = 0;
            Begin = null;
            End = null;
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

        public bool TryGetNext(byte x, out byte result)
        {
            var predOrSucc = PredOrSucc(x);
            result = 0;
            if (predOrSucc == null)
                return false;
            if (predOrSucc.Value <= x)
            {
                var right = predOrSucc.Right as Leaf;
                if (right != null)
                    result = right.Value;
                else
                    return false;
            }
            else
            {
                result = predOrSucc.Value;
            }

            return true;
        }

        public bool TryGetPrevious(byte x, out byte result)
        {
            var predOrSucc = PredOrSucc(x);
            result = 0;
            if (predOrSucc == null)
                return false;
            if (predOrSucc.Value >= x)
            {
                var left = predOrSucc.Left as Leaf;
                if (left != null)
                    result = left.Value;
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
            Stack<Node> stack = new Stack<Node>();
            Leaf? predOrSucc = null;
            int i;
            for (i = 0; i < Dimension - 1; i++)
            {
                stack.Push(current);
                if (((x >> (Dimension - 1 - i)) & 1U) == 1U)
                {
                    if (!(current.Right is InternalNode))
                    {
                        predOrSucc ??= current.Right as Leaf;
                        current.Right = new InternalNode();
                    }

                    current = current.Right;
                }
                else
                {
                    if (!(current.Left is InternalNode))
                    {
                        predOrSucc ??= current.Left as Leaf;
                        current.Left = new InternalNode();
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
            i--;
            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (((x >> (Dimension - 1 - i)) & 1U) == 1U)
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
                i--;
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
                if (((x >> (Dimension - 1 - stack.Count)) & 1U) == 1U)
                {
                    if (destroy)
                        node.Right = null;
                    var leaf = node.Left as Leaf;
                    if (leaf != null && leaf.Value == removed.Value)
                        node.Left = leaf.Right;
                }
                else
                {
                    if (destroy)
                        node.Left = null;
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
