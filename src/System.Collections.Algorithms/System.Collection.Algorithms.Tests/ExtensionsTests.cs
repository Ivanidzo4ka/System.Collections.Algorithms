using System.Collections.Generic;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void GivenArrayWhenPartitionByElementThenAllElementSmallersThanElementInLeftPart()
        {
            int[] data = new int[] { 4, 1, 8, 5, 3, 9, 6 };
            var element = 9;
            var pos = data.Partition(0, data.Length, element);
            int l = 0;
            while (data[l] < element)
                l++;
            Assert.Equal(l, pos);
            Assert.Equal(element, data[l]);
            l++;
            while (l < data.Length)
            {
                Assert.True(data[l] >= element);
                l++;
            }
        }

        [Fact]
        public void GivenListWhenPartitionByElementThenAllElementSmallersThanElementInLeftPart()
        {
            var data = new List<int>() { 4, 1, 8, 5, 3, 9, 6 };
            var element = 6;
            var pos = data.Partition(0, data.Count, element);
            int l = 0;
            while (data[l] < element)
                l++;
            Assert.Equal(l, pos);
            Assert.Equal(element, data[l]);
            l++;
            while (l < data.Count)
            {
                Assert.True(data[l] >= element);
                l++;
            }
        }

        [Fact]
        public void GivenArrayWithDuplicateWhenPartitionByDuplicateElementThenAllElementsSmallerThanElementInLeftPart()
        {
            int[] data = new int[] { 4, 1, 8, 4, 3, 9, 6 };
            var element = 4;
            var pos = data.Partition(0, data.Length, element);
            int l = 0;
            while (data[l] < element)
                l++;
            Assert.Equal(l, pos);
            Assert.Equal(element, data[l]);
            l++;
            while (l < data.Length)
            {
                Assert.True(data[l] >= element);
                l++;
            }
        }

        [Fact]
        public void GivenArrayAndReverseComparerWhenPartitionByElementThenAllElementsSmallerThanElementInRightPart()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            int[] data = new int[] { 4, 1, 8, 5, 3, 9, 6 };
            var element = 6;
            var pos = data.Partition(0, data.Length, element, comparer);
            int r = data.Length - 1;
            while (data[r] < element)
                r--;
            Assert.Equal(r, pos);
            Assert.Equal(element, data[r]);
            r--;
            while (r > 0)
            {
                Assert.True(data[r] >= element);
                r--;
            }
        }

        [Fact]
        public void GivenArrayWithDuplicateAndReverseComparerWhenPartitionByDuplicateElementThenAllElementsSmallerThanElementInRightPart()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            int[] data = new int[] { 4, 1, 8, 4, 3, 9, 6 };
            var element = 6;
            var pos = data.Partition(0, data.Length, element, comparer);
            int r = data.Length - 1;
            while (data[r] < element)
                r--;
            Assert.Equal(r, pos);
            Assert.Equal(element, data[r]);
            r--;
            while (r > 0)
            {
                Assert.True(data[r] >= element);
                r--;
            }
        }

        [Fact]
        public void GivenArrayWhenFindKthElementThenSameAsKthElementInSortedArray()
        {
            var data = new int[] { 4, 1, 8, 4, 3, 9, 6 };
            var sortedData = new int[data.Length];
            Array.Copy(data, sortedData, data.Length);
            Array.Sort(sortedData);
            for (int i = 0; i < data.Length; i++)
            {
                var d = data.KthElement(i);
                Assert.Equal(sortedData[i], d);
            }
        }

        [Fact]
        public void GivenArrayAndComparerWhenFindKthElementThenSameAsKthElementInSortedArray()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var data = new int[] { 4, 1, 8, 4, 3, 9, 6, 7, 7, 10, 2 };
            var sortedData = new int[data.Length];
            Array.Copy(data, sortedData, data.Length);
            Array.Sort(sortedData, comparer);
            for (int i = 0; i < data.Length; i++)
            {
                var d = data.KthElement(i, comparer);
                Assert.Equal(sortedData[i], d);
            }
        }

        [Fact]
        public void GivenArrayWehnFindKthElementThenArrayDidntChanged()
        {
            var data = new int[] { 4, 1, 8, 4, 3, 9, 6 };
            var deepCopy = new int[data.Length];
            Array.Copy(data, deepCopy, data.Length);
            data.KthElement(6);
            Assert.Equal(data, deepCopy);
        }

        [Fact]
        public void GivenMultipleRandomDataWhenFindKthElementThenSameAsKthElementInSortedArray()
        {
            var rand = new Random();
            var n = 100;
            var loops = 1000;
            var data = new int[n];
            var sortedData = new int[n];
            for (int loop = 0; loop < loops; loop++)
            {
                for (int i = 0; i < n; i++)
                    data[i] = rand.Next();
                Array.Copy(data, sortedData, data.Length);
                Array.Sort(sortedData);
                for (int i = 0; i < data.Length; i++)
                {
                    var d = data.KthElement(i);
                    Assert.Equal(sortedData[i], d);
                }
            }
        }

        [Fact]
        public void GivenMultipleRandomDataAndComparerWhenFindKthElementThenSameAsKthElementInSortedArray()
        {
            var comparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var rand = new Random();
            var n = 100;
            var loops = 1000;
            var data = new int[n];
            var sortedData = new int[n];
            for (int loop = 0; loop < loops; loop++)
            {
                for (int i = 0; i < n; i++)
                    data[i] = rand.Next();
                Array.Copy(data, sortedData, data.Length);
                Array.Sort(sortedData, comparer);
                for (int i = 0; i < data.Length; i++)
                {
                    var d = data.KthElement(i, comparer);
                    Assert.Equal(sortedData[i], d);
                }
            }
        }
    }
}
