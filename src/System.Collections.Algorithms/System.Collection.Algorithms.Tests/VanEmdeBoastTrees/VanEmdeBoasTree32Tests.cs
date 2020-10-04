using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests.VanEmdeBoasTrees
{
    public class VanEmdeBoasTree32Tests
    {
        [Fact]
        public void GivenVebTreePopulatedWithOddNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree32();
            for (uint i = 0; i < 1 << 8; i++)
                if (i % 2 == 1)
                    tree.Add(i);
            for (uint i = 0; i < 1 << 8; i++)
                Assert.Equal(i % 2 == 1, tree.Find(i));
        }

        [Fact]
        public void GivenVebTreePopulatedWithEvenNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree32();
            for (uint i = 0; i < 1 << 8; i++)
                if (i % 2 == 0)
                    tree.Add(i);
            for (uint i = 0; i < 1 << 4; i++)
                Assert.Equal(i % 2 == 0, tree.Find(i));
        }

        [Fact]
        public void GivenEmptyVebTreeWhenFindThenAlwaysReturnFalse()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random();
            for (uint i = 0; i < 1 << 8; i++)
            {
                var elem = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
                Assert.False(tree.Find(elem));
            }
        }

        [Fact]
        public void GivenTreeWhenPopulateItThenCountReturnsAmountOfAddedElements()
        {
            var tree = new VanEmdeBoasTree32();
            Assert.Equal(0U, tree.Count);
            for (uint i = 0; i < 1 << 17; i++)
            {
                tree.Add(i);
                Assert.Equal(i + 1, tree.Count);
            }
        }

        [Fact]
        public void GivenRandomCollectionWhenAddingElementsToTreeThenAddReturnsSameResultAsAddingToSet()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random(1);
            var set = new HashSet<uint>();
            for (int i = 0; i < 1 << 17; i++)
            {
                var elem = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
                var add = set.Add(elem);
                Assert.Equal(add, tree.Add(elem));
            }
        }
        [Fact]
        public void GivenPopulatedVebTreeWhenFindThenAlwaysReturnTrue()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random();
            var set = new HashSet<uint>();
            for (int i = 0; i < 1 << 10; i++)
            {
                var elem = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
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
            var tree = new VanEmdeBoasTree32();
            for (uint i = 0; i < 1 << 17; i++)
            {
                Assert.True(tree.Add(i));
                Assert.False(tree.Add(i));
            }

            tree = new VanEmdeBoasTree32();
            for (uint i = (1 << 18) - 1; i > 0; i--)
            {
                Assert.True(tree.Add(i));
                Assert.False(tree.Add(i));
            }
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenTryGetNextReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random();
            var list = new List<uint>();
            for (int i = 0; i < 1 << 17; i++)
            {
                var elem = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
                list.Add(elem);
                tree.Add(elem);
            }
            list.Sort();
            var arr = list.OrderBy(x => x).Distinct().ToArray();
            if (arr[0] != 0)
            {
                Assert.True(tree.TryGetNext(0, out uint res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i] - arr[i - 1] > 1)
                {
                    Assert.True(tree.TryGetNext((arr[i] - arr[i - 1]) / 2 + arr[i - 1], out uint next));
                    Assert.Equal(arr[i], next);
                }
                Assert.True(tree.TryGetNext(arr[i - 1], out uint res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetNext(arr[arr.Length - 1], out uint _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetNextReturnsFalse()
        {
            var tree = new VanEmdeBoasTree32();
            Assert.False(tree.TryGetNext(uint.MinValue, out uint _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetPrevReturnsFalse()
        {
            var tree = new VanEmdeBoasTree32();
            Assert.False(tree.TryGetPrevious(uint.MaxValue, out uint _));
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenGetPrevReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random();
            var list = new List<uint>();
            for (int i = 0; i < 1 << 17; i++)
            {
                var elem = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
                list.Add(elem);
                tree.Add(elem);
            }
            var arr = list.OrderByDescending(x => x).Distinct().ToArray();
            if (arr[0] != uint.MaxValue)
            {
                Assert.True(tree.TryGetPrevious(uint.MaxValue, out uint res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i - 1] - arr[i] > 1)
                {
                    Assert.True(tree.TryGetPrevious((arr[i - 1] - arr[i]) / 2 + arr[i], out uint prev));
                    Assert.Equal(arr[i], prev);
                }
                Assert.True(tree.TryGetPrevious(arr[i - 1], out uint res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetPrevious(arr[arr.Length - 1], out uint _));
        }

        [Fact]
        public void GivenFullVebTreeWhenRemoveNumberThenFindCantFindIt()
        {
            var tree = new VanEmdeBoasTree32();
            for (uint i = 0; i < 1 << 17; i++)
            {
                tree.Add(i);
            }
            for (uint i = 0; i < 1 << 17; i++)
            {
                tree.Remove(i);
                Assert.False(tree.Find(i));
            }
            for (uint i = 0; i < 1 << 17; i++)
            {
                tree.Add(i);
            }
            for (uint i = 0; i < 1 << 17; i++)
            {
                var toRemove = (1 << 17) - 1 - i;
                tree.Remove(toRemove);
                Assert.False(tree.Find(toRemove));
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenRemoveThenMatchesArrayState()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random();
            var arr = new bool[1 << 17];
            for (int i = 0; i < 1 << 7; i++)
            {
                var elem = (uint)rand.Next(1 << 17);
                arr[elem] = true;
                tree.Add(elem);
            }

            for (int i = 0; i < 1 << 7; i++)
            {
                var elem = (uint)rand.Next(1 << 17);
                Assert.Equal(arr[elem], tree.Remove(elem));
                arr[elem] = false;
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenCallMaxAndMinThenReturnProperValues()
        {
            var tree = new VanEmdeBoasTree32();
            var rand = new Random();
            uint min = uint.MaxValue;
            uint max = uint.MinValue;
            for (int i = 0; i < 1000; i++)
            {
                var value = (uint)rand.Next();
                if (min > value) min = value;
                if (max < value) max = value;
                tree.Add(value);
            }
            Assert.Equal(min, tree.Min);
            Assert.Equal(max, tree.Max);
        }

    }
}
