using System;
using System.Collections.Algorithms;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.Collection.Algorithms.Tests
{
    public class Treap
    {
        [Fact]
        public void GivenEmptyTreapWhenAddElementsThenElementsAdded()
        {
            var treap = new Treap<int>();
            treap.Add(1);
            treap.Add(2);
            treap.Add(3);
            treap.Add(4);

            treap.Remove(3);
        }
    }
}
