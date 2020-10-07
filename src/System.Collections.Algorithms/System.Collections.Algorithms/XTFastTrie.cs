using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Algorithms
{
    public class XFastTrie
    {
        private Node _root;
        private const int Dimension = 8;
        public XFastTrie()
        {
            _root = new InternalNode();
        }

        public abstract class Node
        {
            public Node? Left;
            public Node? Right;
            public static int i;
        }

        public class InternalNode : Node
        {
        }

        public class Leaf : Node
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
                if (node is null) break;
                current = node;
            }

            if (i == Dimension)
            {
                return current.Right != null ? (Leaf)current.Right : (Leaf)current.Left;
            }
            else
            {
                if (current.Right != null)
                {
                    current = current.Right;
                    i++;
                    while (i <= Dimension)
                    {
                        if (current.Left != null)
                        {
                            current = current.Left;
                        }
                        else
                        {
                            current = current.Right;
                        }
                        i++;
                    }
                }
                else
                {
                    current = current.Left;
                    i++;
                    while (i <= Dimension)
                    {
                        if (current.Right != null)
                        {
                            current = current.Right;
                        }
                        else
                        {
                            current = current.Left;
                        }
                        i++;
                    }
                }
            }
            return (Leaf)current;
        }

        private Leaf? Succ(byte x)
        {
            var predOrSucc = PredOrSucc(x);
            if (predOrSucc == null)
                return null;
            if (predOrSucc.Value < x)
                return (Leaf)predOrSucc.Right;
            else
                return predOrSucc;
        }

        private Leaf? Pred(byte x)
        {
            var predOrSucc = PredOrSucc(x);
            if (predOrSucc == null)
                return null;
            if (predOrSucc.Value > x)
                return (Leaf)predOrSucc.Left;
            else
                return predOrSucc;
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
                    leaf.Left.Right = null;
                }
                else if (leaf.Right!=null)
                {
                    leaf.Right.Left = null;
                }
            }
        }

        private void InsertRightLeaf(Leaf leaf, Leaf addition)
        {
            var temp = leaf.Right;
            leaf.Right = addition;
            addition.Left = leaf;
            addition.Right = temp;
            if (temp != null)
                temp.Left = addition;
        }

        private void InsertLeftLeaf(Leaf leaf, Leaf addition)
        {
            var temp = leaf.Left;
            leaf.Left = addition;
            addition.Right = leaf;
            addition.Left = temp;
            if (temp != null)
                temp.Right = addition;
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
                        else
                            break;
                    }
                }
                else
                {
                    if (!(node.Right is InternalNode))
                    {
                        var currentLeaf = node.Right as Leaf;
                        if (currentLeaf == null || currentLeaf.Value < added.Value)
                            node.Right = added;
                        else
                            break;
                    }
                }
                i--;
            }
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
