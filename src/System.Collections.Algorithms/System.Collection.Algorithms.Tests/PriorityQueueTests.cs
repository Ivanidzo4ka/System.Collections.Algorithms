using System.Collections.Algorithms;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.Collection.Algorithms.Tests
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
        public void GivenEmptyPriorityQueueWhenPeekElementThenThrowInvalidOpearionException()
        {
            var queue = new PriorityQueue<int, int>();
            Assert.Throws<InvalidOperationException>(() => { queue.Peek(); });
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
        public void GivenPriorityQueueWhenDequeuAllElementsThenEachElementIsSmallerOrEqualThanPrevious()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 } });
            var previous = int.MinValue;
            while (queue.Count != 0)
            {
                Assert.True(queue.Peek().Key >= previous);
                previous = queue.Dequeue().Key;
            }
        }

        [Fact]
        public void GivenQueuePopulatedRandomlyeWhenDequeuAllElementsThenEachElementIsSmallerOrEqualThanPrevious()
        {
            var queue = new PriorityQueue<int, int>();
            var rand = new System.Random(42);
            for (int i = 0; i < 10000; i++)
            {
                queue.Enqueue(rand.Next(1000000), 0);
            }
            var previous = int.MinValue;
            while (queue.Count != 0)
            {
                Assert.True(queue.Peek().Key >= previous);
                previous = queue.Dequeue().Key;
            }
        }

        [Fact]
        public void GivenEmptyQueueWhenSerializedThenCanBeDeserialized()
        {
            var queue = new PriorityQueue<int, int>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, queue);
                ms.Flush();
                ms.Position = 0;
                queue = (PriorityQueue<int, int>)binaryFormatter.Deserialize(ms);
            }
            Assert.NotNull(queue);
        }

        [Fact]
        public void GivenNonEmptyQueueWhenSerializedThenCanBeDeserialized()
        {
            var queue = new PriorityQueue<int, int>(new Dictionary<int, int>() { { 1, 1 } });
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, queue);
                ms.Flush();
                ms.Position = 0;
                queue = (PriorityQueue<int, int>)binaryFormatter.Deserialize(ms);
            }
            Assert.NotNull(queue);
            Assert.Equal(1, queue.Peek().Key);
            Assert.Equal(1, queue.Peek().Value);
            Assert.Single(queue);
        }
    }
}
