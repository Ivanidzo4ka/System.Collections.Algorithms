using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests.VanEmdeBoasTrees
{
    public class VanEmdeBoasTree64Tests
    {
        [Fact]
        public void GivenVebTreePopulatedWithOddNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree64();
            for (ulong i = 0; i < 1 << 8; i++)
                if (i % 2 == 1)
                    tree.Add(i);
            for (ulong i = 0; i < 1 << 8; i++)
                Assert.Equal(i % 2 == 1, tree.Find(i));
        }

        [Fact]
        public void GivenVebTreePopulatedWithEvenNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree64();
            for (ulong i = 0; i < 1 << 8; i++)
                if (i % 2 == 0)
                    tree.Add(i);
            for (ulong i = 0; i < 1 << 4; i++)
                Assert.Equal(i % 2 == 0, tree.Find(i));
        }

        [Fact]
        public void GivenEmptyVebTreeWhenFindThenAlwaysReturnFalse()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            for (ulong i = 0; i < 1 << 8; i++)
            {
                var elem = GetRandomUlong(rand);
                Assert.False(tree.Find(elem));
            }
        }

        [Fact]
        public void GivenTreeWhenPopulateItThenCountReturnsAmountOfAddedElements()
        {
            var tree = new VanEmdeBoasTree64();
            Assert.Equal(0U, tree.Count);
            for (ulong i = 0; i < 1 << 17; i++)
            {
                tree.Add(i << 20);
                Assert.Equal(i + 1, tree.Count);
            }
        }

        [Fact]
        public void GivenRandomCollectionWhenAddingElementsToTreeThenAddReturnsSameResultAsAddingToSet()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            var set = new HashSet<ulong>();
            for (int i = 0; i < 1 << 17; i++)
            {
                var elem = GetRandomUlong(rand);
                var add = set.Add(elem);
                Assert.Equal(add, tree.Add(elem));
            }
        }
        [Fact]
        public void GivenPopulatedVebTreeWhenFindThenAlwaysReturnTrue()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            var set = new HashSet<ulong>();
            for (int i = 0; i < 1 << 10; i++)
            {
                var elem = GetRandomUlong(rand);
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
            var tree = new VanEmdeBoasTree64();
            for (ulong i = 0; i < 1 << 10; i++)
            {
                Assert.True(tree.Add(i << 20));
                Assert.False(tree.Add(i << 20));
            }

            tree = new VanEmdeBoasTree64();
            for (ulong i = (1 << 18) - 1; i > 0; i--)
            {
                Assert.True(tree.Add(i << 20));
                Assert.False(tree.Add(i << 20));
            }
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenTryGetNextReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            var list = new List<ulong>();
            for (int i = 0; i < 1 << 17; i++)
            {
                var elem = GetRandomUlong(rand);
                list.Add(elem);
                tree.Add(elem);
            }
            list.Sort();
            var arr = list.OrderBy(x => x).Distinct().ToArray();
            if (arr[0] != 0)
            {
                Assert.True(tree.TryGetNext(0, out ulong res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i] - arr[i - 1] > 1)
                {
                    Assert.True(tree.TryGetNext((arr[i] - arr[i - 1]) / 2 + arr[i - 1], out ulong next));
                    Assert.Equal(arr[i], next);
                }
                Assert.True(tree.TryGetNext(arr[i - 1], out ulong res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetNext(arr[arr.Length - 1], out ulong _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetNextReturnsFalse()
        {
            var tree = new VanEmdeBoasTree64();
            Assert.False(tree.TryGetNext(ulong.MinValue, out ulong _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetPrevReturnsFalse()
        {
            var tree = new VanEmdeBoasTree64();
            Assert.False(tree.TryGetPrevious(ulong.MaxValue, out ulong _));
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenGetPrevReturnsCorrectValue()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            var list = new List<ulong>();
            for (int i = 0; i < 1 << 17; i++)
            {
                var elem = GetRandomUlong(rand);
                list.Add(elem);
                tree.Add(elem);
            }
            var arr = list.OrderByDescending(x => x).Distinct().ToArray();
            if (arr[0] != ulong.MaxValue)
            {
                Assert.True(tree.TryGetPrevious(ulong.MaxValue, out ulong res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i - 1] - arr[i] > 1)
                {
                    Assert.True(tree.TryGetPrevious((arr[i - 1] - arr[i]) / 2 + arr[i], out ulong prev));
                    Assert.Equal(arr[i], prev);
                }
                Assert.True(tree.TryGetPrevious(arr[i - 1], out ulong res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(tree.TryGetPrevious(arr[arr.Length - 1], out ulong _));
        }

        [Fact]
        public void GivenFullVebTreeWhenRemoveNumberThenFindCantFindIt()
        {
            var tree = new VanEmdeBoasTree64();
            for (ulong i = 0; i < 1 << 17; i++)
            {
                tree.Add(i << 20);
            }
            for (ulong i = 0; i < 1 << 17; i++)
            {
                tree.Remove(i << 20);
                Assert.False(tree.Find(i));
            }
            for (ulong i = 0; i < 1 << 17; i++)
            {
                tree.Add(i << 20);
            }
            for (ulong i = 0; i < 1 << 17; i++)
            {
                var toRemove = (1 << 37) - 1 - (i << 20);
                tree.Remove(toRemove);
                Assert.False(tree.Find(toRemove));
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenRemoveThenMatchesArrayState()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            var arr = new bool[1 << 17];
            for (int i = 0; i < 1 << 7; i++)
            {
                var elem = (ulong)rand.Next(1 << 17);
                arr[elem] = true;
                tree.Add(elem);
            }

            for (int i = 0; i < 1 << 7; i++)
            {
                var elem = (ulong)rand.Next(1 << 17);
                Assert.Equal(arr[elem], tree.Remove(elem));
                arr[elem] = false;
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenCallMaxAndMinThenReturnProperValues()
        {
            var tree = new VanEmdeBoasTree64();
            var rand = new Random();
            ulong min = ulong.MaxValue;
            ulong max = ulong.MinValue;
            for (int i = 0; i < 1000; i++)
            {
                var value = GetRandomUlong(rand);
                if (min > value) min = value;
                if (max < value) max = value;
                tree.Add(value);
            }
            Assert.Equal(min, tree.Min);
            Assert.Equal(max, tree.Max);
        }

        private ulong GetRandomUlong(Random rand)
        {
            var upper = ((ulong)rand.Next(1 << 30) << 31);
            var lower = (ulong)rand.Next(1 << 30);
            return upper | lower;
        }
    }
}
