using System;
using System.Collections.Algorithms;
using System.Collections.Generic;
using System.Linq;
using Xunit;
namespace System.Collections.Algorithms.Tests.VebTree
{
    public class VebTree16Tests
    {
        [Fact]
        public void GivenVebTreePopulatedWithOddNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree16();
            for (int i = 0; i <= ushort.MaxValue; i++)
                if (i % 2 == 1)
                    tree.Add((ushort)i);
            for (int i = 0; i <= ushort.MaxValue; i++)
                Assert.Equal(i % 2 == 1, tree.Find((ushort)i));
        }

        [Fact]
        public void GivenVebTreePopulatedWithEvenNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree16();
            for (int i = 0; i <= ushort.MaxValue; i++)
                if (i % 2 == 0)
                    tree.Add((ushort)i);
            for (int i = 0; i <= ushort.MaxValue; i++)
                Assert.Equal(i % 2 == 0, tree.Find((ushort)i));
        }

        [Fact]
        public void GivenEmptyVebTreeWhenFindThenAlwaysReturnFalse()
        {
            var tree = new VebTree16();
            var rand = new Random();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 8);
                Assert.False(tree.Find(elem));
            }
        }

        [Fact]
        public void GivenTreeWhenPopulateItThenCountReturnsAmountOfAddedElements()
        {
            var tree = new VebTree16();
            Assert.Equal(0U, tree.Count);
            for (uint i = 0; i <= ushort.MaxValue; i++)
            {
                tree.Add((ushort)i);
                Assert.Equal(i + 1, tree.Count);
            }
        }

        [Fact]
        public void GivenRandomCollectionWhenAddingElementsToTreeThenAddReturnsSameResultAsAddingToSet()
        {
            var tree = new VebTree16();
            var rand = new Random();
            var set = new HashSet<uint>();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 16);
                var add = set.Add(elem);
                Assert.Equal(add, tree.Add(elem));
            }
        }
        [Fact]
        public void GivenPopulatedVebTreeWhenFindThenAlwaysReturnTrue()
        {
            var tree = new VebTree16();
            var rand = new Random();
            var set = new HashSet<ushort>();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 16);
                set.Add(elem);
                tree.Add(elem);
            }
            foreach (var elem in set)
            {
                Assert.True(tree.Find(elem));
            }
        }

        [Fact]
        public void GivenVebTreeWhenAddSameNumberThenReturnFalse()
        {
            var tree = new VebTree16();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                Assert.True(tree.Add((ushort)i));
                Assert.False(tree.Add((ushort)i));
            }

            tree = new VebTree16();
            for (int i = ushort.MaxValue; i > 0; i--)
            {
                Assert.True(tree.Add((ushort)i));
                Assert.False(tree.Add((ushort)i));
            }
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenTryGetNextReturnsCorrectValue()
        {
            var tree = new VebTree16();
            var rand = new Random();
            var list = new List<ushort>();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 16);
                list.Add(elem);
                tree.Add(elem);
            }
            list.Sort();
            var arr = list.OrderBy(x => x).Distinct().ToArray();
            if (arr[0] != 0)
            {
                Assert.True(tree.TryGetNext(0, out ushort res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i] - arr[i - 1] > 1)
                {
                    Assert.True(tree.TryGetNext((ushort)((arr[i] - arr[i - 1]) / 2 + arr[i - 1]), out ushort next));
                    Assert.Equal(arr[i], next);
                }
                Assert.True(tree.TryGetNext(arr[i - 1], out ushort res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetNext(arr[arr.Length - 1], out ushort _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetNextReturnsFalse()
        {
            var tree = new VebTree16();
            Assert.False(tree.TryGetNext(ushort.MinValue, out ushort _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetPrevReturnsFalse()
        {
            var tree = new VebTree16();
            Assert.False(tree.TryGetPrevious(ushort.MaxValue, out ushort _));
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenGetPrevReturnsCorrectValue()
        {
            var tree = new VebTree16();
            var rand = new Random();
            var list = new List<ushort>();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 16);
                list.Add(elem);
                tree.Add(elem);
            }
            var arr = list.OrderByDescending(x => x).Distinct().ToArray();
            if (arr[0] != ushort.MaxValue)
            {
                Assert.True(tree.TryGetPrevious(ushort.MaxValue, out ushort res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i - 1] - arr[i] > 1)
                {
                    Assert.True(tree.TryGetPrevious((ushort)((arr[i - 1] - arr[i]) / 2 + arr[i]), out ushort prev));
                    Assert.Equal(arr[i], prev);
                }
                Assert.True(tree.TryGetPrevious(arr[i - 1], out ushort res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetPrevious(arr[arr.Length - 1], out ushort _));
        }

        [Fact]
        public void GivenFullVebTreeWhenRemoveNumberThenFindCantFindIt()
        {
            var tree = new VebTree16();
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                tree.Add((ushort)i);
            }
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                tree.Remove((ushort)i);
                Assert.False(tree.Find((ushort)i));
            }
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                tree.Add((ushort)i);
            }
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var toRemove = (ushort)(ushort.MaxValue - i);
                tree.Remove(toRemove);
                Assert.False(tree.Find(toRemove));
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenRemoveThenMatchesArrayState()
        {
            var tree = new VebTree16();
            var rand = new Random();
            var arr = new bool[1 << 16];
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 16);
                arr[elem] = true;
                tree.Add(elem);
            }

            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                var elem = (ushort)rand.Next(1 << 16);
                Assert.Equal(arr[elem], tree.Remove(elem));
                arr[elem] = false;
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenCallMaxAndMinThenReturnProperValues()
        {
            var tree = new VebTree16();
            var rand = new Random();
            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;
            for (int i = 0; i < 1000; i++)
            {
                var value = (ushort)rand.Next(1 << 16);
                if (min > value) min = value;
                if (max < value) max = value;
                tree.Add(value);
            }
            Assert.Equal(min, tree.Min);
            Assert.Equal(max, tree.Max);
        }

    }
}