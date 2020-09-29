using System.Collections.Generic;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class PriorityQueueTests
    {
        [Fact]
        public void GivenNothingWhenCreatePriorityQueueThenQueueCreated()
        {
            var queue = new PriorityQueue<int, int>();
            Assert.NotNull(queue);
        }

        [Fact]
        public void GivenCapacityWhenCreatePriorityQueueThenQueueCreated()
        {
            var queue = new PriorityQueue<int, int>(100);
            Assert.NotNull(queue);
        }

        [Fact]
        public void GivenComparerWhenCreatePriorityQueueThenQueueCreated()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var queue = new PriorityQueue<int, int>(comparer);
            Assert.NotNull(queue);
        }

        [Fact]
        public void GivenComparerAndCapacityWhenCreatePriorityQueueThenQueueCreated()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var queue = new PriorityQueue<int, int>(100, comparer);
            Assert.NotNull(queue);
        }

        [Fact]
        public void GivenNullCollectionWhenTryCreateQueueThenThrowsAgrumentException()
        {
            Assert.Throws<ArgumentNullException>(() => { var queue = new PriorityQueue<int, int>((IEnumerable<KeyValuePair<int, int>>)null); });
        }

        [Fact]
        public void GivenEmptyPriorityQueueWhenDequeueElementThenThrowInvalidOpearionException()
        {
            var queue = new PriorityQueue<int, int>();
            Assert.Throws<InvalidOperationException>(() => { queue.Dequeue(); });
        }

        [Fact]
        public void GivenEmptyPriorityQueueWhenTryDequeueElementThenReturnFalse()
        {
            var queue = new PriorityQueue<int, int>();
            KeyValuePair<int, int> item;
            Assert.False(queue.TryDequeue(out item));
            Assert.Equal(default(KeyValuePair<int, int>), item);
        }

        [Fact]
        public void GivenCollectionQueueWhenTryDequeueElementThenReturnTrue()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 100 }, { 2, 200 } });
            KeyValuePair<int, int> item;
            Assert.True(queue.TryDequeue(out item));
            Assert.Equal(1, item.Key);
            Assert.Equal(100, item.Value);
        }

        [Fact]
        public void GIvenEmptyQueueWhenCheckingIsEmptyShouldReturnTrue()
        {
            var queue = new PriorityQueue<int, int>();
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public void GivenEmptyPriorityQueueWhenPeekElementThenThrowInvalidOpearionException()
        {
            var queue = new PriorityQueue<int, int>();
            Assert.Throws<InvalidOperationException>(() => { queue.Peek(); });
        }

        [Fact]
        public void GivenEmptyPriorityQueueWhenTryPeekElementThenReturnFalse()
        {
            var queue = new PriorityQueue<int, int>();
            KeyValuePair<int, int> item;
            Assert.False(queue.TryPeek(out item));
            Assert.Equal(default(KeyValuePair<int, int>), item);
        }

        [Fact]
        public void GivenCollectionQueueWhenTryPeekElementThenReturnTrue()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 100 }, { 2, 200 } });
            KeyValuePair<int, int> item;
            Assert.True(queue.TryPeek(out item));
            Assert.Equal(1, item.Key);
            Assert.Equal(100, item.Value);
        }

        [Fact]
        public void GivenNonEmptyCollectionWhenDequeueThenReturnsMinElementAndRemovesElementAndReduceCount()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } });
            var topItem = queue.Dequeue();
            Assert.Equal(1, topItem.Key);
            Assert.Equal(1, topItem.Value);
            Assert.Single(queue);
        }

        [Fact]
        public void GivenNonEmptyCollectionWhenPeekThenReturnsMinElementAndDoesntChangeCount()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } });
            var topItem = queue.Peek();
            Assert.Equal(1, topItem.Key);
            Assert.Equal(1, topItem.Value);
            Assert.Equal(2, queue.Count);
        }


        [Fact]
        public void GivenNonEmptyCollectionWhenSpecifyMaxComparerThenReturnsMaxElement()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } }, comparer);
            var topItem = queue.Peek();
            Assert.Equal(2, topItem.Key);
            Assert.Equal(2, topItem.Value);
            Assert.Equal(2, queue.Count);
        }

        [Fact]
        public void GivenPriorityQueueWhenDequeuAllElementsThenEachElementIsSmallerOrEqualThanPrevious()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 } });
            var previous = int.MinValue;
            while (!queue.IsEmpty)
            {
                Assert.True(queue.Peek().Key >= previous);
                previous = queue.Dequeue().Key;
            }
        }

        [Fact]
        public void GivenQueuePopulatedRandomlyeWhenDequeuAllElementsThenEachElementIsSmallerOrEqualThanPrevious()
        {
            var queue = new PriorityQueue<int, int>();
            var rand = new Random(42);
            for (int i = 0; i < 10000; i++)
            {
                queue.Enqueue(rand.Next(1000000), 0);
            }
            var previous = int.MinValue;
            while (!queue.IsEmpty)
            {
                Assert.True(queue.Peek().Key >= previous);
                previous = queue.Dequeue().Key;
            }
        }

        [Fact]
        public void GivenQueueWithElementWhenCallContainsValueForElementThenReturnsTrue()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 0, 1 } });
            Assert.True(queue.ContainsValue(1));
        }

        [Fact]
        public void GivenQueueWithNoElementWhenCallContainsValueForElementThenReturnsFalse()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 0, 1 } });
            Assert.False(queue.ContainsValue(0));
        }

        [Fact]
        public void GivenQueueWithElementWhenCallRemoveValueForElementThenReturnsTrue()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 0, 1 } });
            Assert.True(queue.Remove(1));
            Assert.Empty(queue);
        }
    }
}
