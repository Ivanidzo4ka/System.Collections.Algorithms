using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Algorithms
{
    public class XFastTrie
    {
        private Node _root;
        private const int Dimension = 32;
        public XFastTrie()
        {
            _root = new InternalNode();
        }

        public abstract class Node
        {
            public Node? Left;
            public Node? Right;
        }

        public class InternalNode : Node
        {
        }

        public class Leaf : Node
        {

            public uint Value;
        }

        private Leaf? PredOrSucc(uint x)
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

        private Leaf? Succ(uint x)
        {
            var predOrSucc = PredOrSucc(x);
            if (predOrSucc == null)
                return null;
            if (predOrSucc.Value < x)
                return (Leaf)predOrSucc.Right;
            else
                return predOrSucc;
        }

        private Leaf? Pred(uint x)
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
                else
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

        public bool Add(uint x)
        {
            Node current = _root;
            Leaf? predOrSucc = PredOrSucc(x);
            int i;
            for (i = 0; i < Dimension; i++)
            {
                if (((x >> (Dimension - 1 - i)) & 1U) == 1U)
                {
                    if (current.Right is null)
                    {
                        current.Right = new InternalNode();
                    }

                    current = current.Right;
                }
                else
                {
                    if (current.Left is null)
                    {
                        current.Left = new InternalNode();
                    }
                    current = current.Left;
                }
            }
            if (((x >> i) & 1U) == 1U)
            {
                if (current.Right is null)
                {
                    var addition = new Leaf() { Value = x };
                    current.Right = addition;
                    if (predOrSucc != null)
                        if (predOrSucc.Value < x)
                            InsertRightLeaf(predOrSucc, addition);
                        else
                            InsertLeftLeaf(predOrSucc, addition);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (current.Left is null)
                {
                    var addition = new Leaf() { Value = x };
                    current.Left = addition;
                    if (predOrSucc != null)
                        if (predOrSucc.Value > x)
                            InsertLeftLeaf(predOrSucc, addition);
                        else
                            InsertRightLeaf(predOrSucc, addition);
                    return true;
                }
                else
                    return false;
            }
        }

        public bool Find(uint x)
        {
            Node? current = _root;
            for (int i = 0; i < Dimension; i++)
            {
                current = ((x >> (Dimension - 1 - i)) & 1U) == 1U ? current.Right : current.Left;
                if (current is null) return false;
            }

            return true;
        }

        public bool Remove(uint x)
        {
            Node? current = _root;
            int i;
            Stack<Node> stack = new Stack<Node>();
            for (i = 0; i < Dimension ; i++)
            {
                current = ((x >> (Dimension - 1 - i)) & 1U) == 1U ? current.Right : current.Left;
                if (current is null) return false;
                stack.Push(current);
            }
            if (((x >> i) & 1U) == 1U)
            {
                RemoveLeaf((Leaf)current.Right);
                current.Right = null;
            }
            else
            {
                RemoveLeaf((Leaf)current.Left);
                current.Left = null;
            }
            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (((x >> (Dimension - 1 - i)) & 1U) == 1U)
                    node.Right = null;
                else
                    node.Left = null;

                if (node.Left != null || node.Right != null) break;
                i--;
            }
            return true;
        }
    }
}
