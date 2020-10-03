using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests.VebTree
{
    public class VebTree8Tests
    {
        [Fact]
        public void GivenVebTreePopulatedWithOddNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree8();
            for (int i = 0; i <= byte.MaxValue; i++)
                if (i % 2 == 1)
                    tree.Add((byte)i);
            for (int i = 0; i <= byte.MaxValue; i++)
                Assert.Equal(i % 2 == 1, tree.Find((byte)i));
        }

        [Fact]
        public void GivenVebTreePopulatedWithEvenNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree8();
            for (int i = 0; i <= byte.MaxValue; i++)
                if (i % 2 == 0)
                    tree.Add((byte)i);
            for (int i = 0; i <= byte.MaxValue; i++)
                Assert.Equal(i % 2 == 0, tree.Find((byte)i));
        }

        [Fact]
        public void GivenEmptyVebTreeWhenFindThenAlwaysReturnFalse()
        {
            var tree = new VebTree8();
            var rand = new Random();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                Assert.False(tree.Find(elem));
            }
        }

        [Fact]
        public void GivenTreeWhenPopulateItThenCountReturnsAmountOfAddedElements()
        {
            var tree = new VebTree8();
            Assert.Equal(0, tree.Count);
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                tree.Add((byte)i);
                Assert.Equal(i + 1, tree.Count);
            }
        }

        [Fact]
        public void GivenRandomCollectionWhenAddingElementsToTreeThenAddReturnsSameResultAsAddingToSet()
        {
            var tree = new VebTree8();
            var rand = new Random();
            var set = new HashSet<uint>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                var add = set.Add(elem);
                Assert.Equal(add, tree.Add(elem));
            }
        }
        [Fact]
        public void GivenPopulatedVebTreeWhenFindThenAlwaysReturnTrue()
        {
            var tree = new VebTree8();
            var rand = new Random();
            var set = new HashSet<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
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
            var tree = new VebTree8();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                Assert.True(tree.Add((byte)i));
                Assert.False(tree.Add((byte)i));
            }

            tree = new VebTree8();
            for (int i = byte.MaxValue; i > 0; i--)
            {
                Assert.True(tree.Add((byte)i));
                Assert.False(tree.Add((byte)i));
            }
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenTryGetNextReturnsCorrectValue()
        {
            var tree = new VebTree8();
            var rand = new Random();
            var list = new List<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                list.Add(elem);
                tree.Add(elem);
            }
            list.Sort();
            var arr = list.OrderBy(x => x).Distinct().ToArray();
            if (arr[0] != 0)
            {
                Assert.True(tree.TryGetNext(0, out byte res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i] - arr[i - 1] > 1)
                {
                    Assert.True(tree.TryGetNext((byte)((arr[i] - arr[i - 1]) / 2 + arr[i - 1]), out byte next));
                    Assert.Equal(arr[i], next);
                }
                Assert.True(tree.TryGetNext(arr[i - 1], out byte res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetNext(arr[arr.Length - 1], out byte _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetNextReturnsFalse()
        {
            var tree = new VebTree8();
            Assert.False(tree.TryGetNext(byte.MinValue, out byte _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetPrevReturnsFalse()
        {
            var tree = new VebTree8();
            Assert.False(tree.TryGetPrevious(byte.MaxValue, out byte _));
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenGetPrevReturnsCorrectValue()
        {
            var tree = new VebTree8();
            var rand = new Random();
            var list = new List<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                list.Add(elem);
                tree.Add(elem);
            }
            var arr = list.OrderByDescending(x => x).Distinct().ToArray();
            if (arr[0] != byte.MaxValue)
            {
                Assert.True(tree.TryGetPrevious(byte.MaxValue, out byte res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i - 1] - arr[i] > 1)
                {
                    Assert.True(tree.TryGetPrevious((byte)((arr[i - 1] - arr[i]) / 2 + arr[i]), out byte prev));
                    Assert.Equal(arr[i], prev);
                }
                Assert.True(tree.TryGetPrevious(arr[i - 1], out byte res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetPrevious(arr[arr.Length - 1], out byte _));
        }

        [Fact]
        public void GivenFullVebTreeWhenRemoveNumberThenFindCantFindIt()
        {
            var tree = new VebTree8();
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                tree.Add((byte)i);
            }
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                tree.Remove((byte)i);
                Assert.False(tree.Find((byte)i));
            }
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                tree.Add((byte)i);
            }
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                var toRemove = (byte)(byte.MaxValue - i);
                tree.Remove(toRemove);
                Assert.False(tree.Find(toRemove));
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenRemoveThenMatchesArrayState()
        {
            for (int randIter = 0; randIter < 1000; randIter++)
            {
                var tree = new VebTree8();
                var rand = new Random();
                var arr = new bool[1 << 8];
                for (int i = 0; i <= byte.MaxValue; i++)
                {
                    var elem = (byte)rand.Next(1 << 8);
                    arr[elem] = true;
                    tree.Add(elem);
                }

                for (int i = 0; i <= byte.MaxValue; i++)
                {
                    var elem = (byte)rand.Next(1 << 8);
                    Assert.Equal(arr[elem], tree.Remove(elem));
                    arr[elem] = false;
                }
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenCallMaxAndMinThenReturnProperValues()
        {
            var tree = new VebTree8();
            var rand = new Random();
            byte min = byte.MaxValue;
            byte max = byte.MinValue;
            for (int i = 0; i < 1000; i++)
            {
                var value = (byte)rand.Next(1 << 8);
                if (min > value) min = value;
                if (max < value) max = value;
                tree.Add(value);
            }
            Assert.Equal(min, tree.Min);
            Assert.Equal(max, tree.Max);
        }

    }
}
