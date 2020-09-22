using System;
using System.Collections.Algorithms;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.Collection.Algorithms.Tests
{
    public class TreapTests
    {
        [Fact]
        public void GivenEmptyTreapWhenAddElementsThenElementsAdded()
        {
            var treap = new Treap<int>();
            treap.Add(1);
            treap.Add(2);
            treap.Add(3);
            treap.Add(4);
            Assert.Equal(1, treap[0]);
            Assert.Equal(2, treap[1]);
            Assert.Equal(3, treap[2]);
            Assert.Equal(4, treap[3]);
        }

        [Fact]
        public void GivenCollectionWhenCreateTreapWithItThenNothingBlewsUp()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection);
            for (int i = 0; i < collection.Count; i++)
            {
                Assert.Equal(collection[i], treap[i]);
            }
        }

        [Fact]
        public void GivenTreapWhenAddingAndRemovingElementsThenCountStaysRight()
        {
            var treap = new Treap<int>();
            Assert.Equal(0, treap.Count);
            treap.Add(4);
            Assert.Equal(1, treap.Count);
            treap.Add(1);
            Assert.Equal(2, treap.Count);
            treap.Remove(4);
            Assert.Equal(1, treap.Count);
            treap.Remove(1);
            Assert.Equal(0, treap.Count);
        }

        [Fact]
        public void GivenCollectionWhenCreateTreapWithItThenCanEnumerate()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection);
            var pos = 0;
            foreach (var item in treap)
            {
                Assert.Equal(collection[pos++], item);
            }
        }
    }
}
