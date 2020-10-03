using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class VebTree32Tests
    {
        [Fact]
        public void GivenVebTreePopulatedWithOddNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree32(4);
            for (uint i = 0; i < 1 << 4; i++)
                if (i % 2 == 1)
                    tree.Add(i);
            for (uint i = 0; i < 1 << 4; i++)
                Assert.Equal(i % 2 == 1, tree.Find(i));
        }

        [Fact]
        public void GivenVebTreePopulatedWithEvenNumbersWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree32(4);
            for (uint i = 0; i < 1 << 4; i++)
                if (i % 2 == 0)
                    tree.Add(i);
            for (uint i = 0; i < 1 << 4; i++)
                Assert.Equal(i % 2 == 0, tree.Find(i));
        }

        [Fact]
        public void GivenEmptyVebTreeWhenFindThenAlwaysReturnFalse()
        {
            var tree = new VebTree32(4);
            for (uint i = 0; i < 1 << 4; i++)
                Assert.False(tree.Find(i));
        }

        [Fact]
        public void GivenFullVebTreeWhenFindThenAlwaysReturnTrue()
        {
            var tree = new VebTree32(4);
            for (uint i = 0; i < 1 << 4; i++)
                tree.Add(i);
            for (uint i = 0; i < 1 << 4; i++)
                Assert.True(tree.Find(i));
        }

        [Fact]
        public void GivenVebTreeWhenAddSameNumberThenReturnFalse()
        {
            var tree = new VebTree32(4);
            for (uint i = 0; i < 1 << 4; i++)
            {
                Assert.True(tree.Add(i));
                Assert.False(tree.Add(i));
            }

            tree = new VebTree32(4);
            for (uint i = (1 << 4) - 1; i > 0; i--)
            {
                Assert.True(tree.Add(i));
                Assert.False(tree.Add(i));
            }

        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenTryGetNextReturnsCorrectValue()
        {
            for (int randIter = 0; randIter < 1000; randIter++)
            {
                var tree = new VebTree32(8);
                var rand = new Random();
                int[] arr = new int[1 << 8];
                for (int i = 0; i < 1 << 7; i++)
                {
                    var elem = (uint)rand.Next(1 << 8);
                    arr[elem] = 1;
                    tree.Add(elem);
                }

                for (uint i = 0; i < 1 << 8; i++)
                {
                    uint j = i + 1;
                    while (j < 1 << 8)
                    {
                        if (arr[j] == 1)
                            break;
                        j++;
                    }
                    uint ans;
                    if (j != 1 << 8)
                    {
                        Assert.True(tree.TryGetNext(i, out ans));
                        Assert.Equal(j, ans);
                    }
                    else
                        Assert.False(tree.TryGetNext(i, out _));
                }
            }
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetNextReturnsFalse()
        {
            var tree = new VebTree32(8);
            Assert.False(tree.TryGetNext(0U, out uint _));
        }

        [Fact]
        public void GivenEmptyTreeWhenTryGetPrevReturnsFalse()
        {
            var tree = new VebTree32(8);
            Assert.False(tree.TryGetPrevious(0U, out uint _));
        }

        [Fact]
        public void GivenRandomPopulatedTreeWhenGetPrevReturnsCorrectValue()
        {
            for (int randIter = 0; randIter < 1000; randIter++)
            {
                var tree = new VebTree32(8);
                var rand = new Random();
                int[] arr = new int[1 << 8];
                for (int i = 0; i < 1 << 7; i++)
                {
                    var elem = (uint)rand.Next(1 << 8);
                    arr[elem] = 1;
                    tree.Add(elem);
                }

                for (int i = 1; i < 1 << 8; i++)
                {
                    int j = i - 1;
                    while (j >= 0)
                    {
                        if (arr[j] == 1)
                            break;
                        j--;
                    }
                    uint ans;
                    if (j != -1)
                    {
                        Assert.True(tree.TryGetPrevious((uint)i, out ans));
                        Assert.Equal((uint)j, ans);
                    }
                    else
                        Assert.False(tree.TryGetPrevious((uint)i, out _));
                }
            }
        }

        [Fact]
        public void GivenFullVebTreeWhenRemoveNumberThenFindCantFindIt()
        {
            var tree = new VebTree32(4);
            for (uint i = 0; i < 1 << 4; i++)
            {
                tree.Add(i);
            }
            for (uint i = 0; i < 1 << 4; i++)
            {
                tree.Remove(i);
                Assert.False(tree.Find(i));
            }
            for (uint i = 0; i < 1 << 4; i++)
            {
                tree.Add(i);
            }
            for (uint i = 0; i < 1 << 4; i++)
            {
                var toRemove = (1 << 4) - 1 - i;
                tree.Remove(toRemove);
                Assert.False(tree.Find(toRemove));
            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenRemoveThenMatchesArrayState()
        {
            for (int randIter = 0; randIter < 1000; randIter++)
            {
                var tree = new VebTree32(8);
                var rand = new Random();
                var arr = new bool[1 << 8];
                for (int i = 0; i < 1 << 7; i++)
                {
                    var elem = (uint)rand.Next(1 << 8);
                    arr[elem] = true;
                    tree.Add(elem);
                }

                for (int i = 0; i < 1 << 7; i++)
                {
                    var elem = (uint)rand.Next(1 << 8);
                    Assert.Equal(arr[elem], tree.Remove(elem));
                    arr[elem] = false;
                }

            }
        }

        [Fact]
        public void GivenRandomFilledTreeWhenCallMaxAndMinThenReturnProperValues()
        {
            var tree = new VebTree32();
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
