using System.Collections.Algorithms;
using System.Collections.Generic;
using System.Linq;
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
        public void GivenNonEmptyCollectionWhenSpecifyMaxComparerThenOrdererMaxToMin()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var collection = new List<int> { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection, comparer);
            Assert.Equal(20, treap[0]);
            Assert.Equal(1, treap[treap.Count - 1]);
        }

        [Fact]
        public void GivenEmptyCollectionWhenSpecifyMaxComparerThenOrdererMaxToMin()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var treap = new Treap<int>(comparer);
            treap.Add(1);
            treap.Add(20);
            treap.Add(10);
            treap.Add(5);
            Assert.Equal(20, treap[0]);
            Assert.Equal(1, treap[treap.Count - 1]);
        }

        [Fact]
        public void GivenCollectionWhenCreateTreapWithItThenIndexerReturnRightValues()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection);
            for (int i = 0; i < collection.Count; i++)
            {
                Assert.Equal(collection[i], treap[i]);
            }
        }

        [Fact]
        public void GivenTreapWhenCallWrongIndexerThenArgumentRangeException()
        {
            var treap = new Treap<int>();
            var list = new List<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => treap[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => treap[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => treap[1]);
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
        public void GivenTreapWhenRemoveThenReturnProperResult()
        {
            var treap = new Treap<int>(new List<int> { 1, 2, 3, 3, 3, 4, 5, 6, 7, 8, 11, 100, 1000 });
            Assert.True(treap.Remove(3));
            Assert.True(treap.Remove(3));
            Assert.True(treap.Remove(3));
            Assert.False(treap.Remove(3));
            Assert.True(treap.Remove(11));
            Assert.True(treap.Remove(1000));
        }

        [Fact]
        public void GivenCollectionWhenCreateTreapWithItThenCanEnumerate()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection);
            Assert.Equal(20, treap.Count);
            var pos = 0;
            foreach (var item in treap)
            {
                Assert.Equal(collection[pos++], item);
            }
        }

        [Fact]
        public void GivenTreapWhenWalkingInReverseOrderGetCorrectResults()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection);
            Assert.Equal(20, treap.Count);
            var pos = collection.Count - 1;
            foreach (var item in treap.Reverse())
            {
                Assert.Equal(collection[pos--], item);
            }
        }

        [Fact]
        public void GivenTreapWhenLookingForExistingElementThenContainsTrue()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            var treap = new Treap<int>(collection);
            foreach (var item in collection)
                Assert.True(treap.Contains(item));
        }

        [Fact]
        public void GivenEmptyTreapWhenCallingContainsThenReturnFalse()
        {
            var treap = new Treap<int>();
            Assert.False(treap.Contains(1));
            Assert.False(treap.Contains(-1));
        }

        [Fact]
        public void GivenTreapWhenLookingForNonExistingElementThenContainsFalse()
        {
            var collection = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var treap = new Treap<int>(collection);
            Assert.False(treap.Contains(0));
            Assert.False(treap.Contains(-1));
            Assert.False(treap.Contains(21));
        }

        [Fact]
        public void GivenTwoTreapsWhenMergeOnIntoAnotherThenFirstTreapHasAllDataAndSecondIsEmpty()
        {
            var a = new List<int> { 1, 5, 10, 15, 20 };
            var b = new List<int> { 2, 6, 11, 16, 21 };
            var first = new Treap<int>(a);
            var second = new Treap<int>(b);
            first.MergeIn(second);
            Assert.Equal(10, first.Count);
            Assert.Equal(0, second.Count);
            Assert.Equal(a.Concat(b).OrderBy(i => i), first);
        }

        [Fact]
        public void GivenTwoTreapsWithDuplicatesWhenMergeOnIntoAnotherThenFirstTreapHasAllDataAndSecondIsEmpty()
        {
            var a = new List<int> { -100, -99, 1, 5, 10, 15, 20, 100, 101 };
            var b = new List<int> { -100, -99, 2, 6, 10, 15, 21, 101, 102 };
            var first = new Treap<int>(a);
            var second = new Treap<int>(b);
            first.MergeIn(second);
            Assert.Equal(a.Count + b.Count, first.Count);
            Assert.Equal(0, second.Count);
            Assert.Equal(a.Concat(b).OrderBy(i => i), first);
        }

        [Fact]
        public void GivenOneEmptyAndOneFullTreapsWhenMergeInThenTreapDontChange()
        {
            var a = new List<int> { 1, 5, 10, 15, 20 };
            var first = new Treap<int>(a);
            var second = new Treap<int>();
            first.MergeIn(second);
            Assert.Equal(5, first.Count);
            Assert.Equal(0, second.Count);
            Assert.Equal(a.OrderBy(i => i), first);

            first = new Treap<int>();
            second = new Treap<int>(a);
            first.MergeIn(second);
            Assert.Equal(5, first.Count);
            Assert.Equal(0, second.Count);
            Assert.Equal(a.OrderBy(i => i), first);
        }
    }
}
