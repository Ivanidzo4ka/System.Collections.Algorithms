using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class XFastTrieTests
    {
        [Fact]
        public void GivenXtriePopulatedWithOddNumbersWhenFindReturnsCorrectValue()
        {
            var trie = new XTFastTrie();
            for (int i = 0; i <= byte.MaxValue; i++)
                if (i % 2 == 1)
                    trie.Add((byte)i);
            for (int i = 0; i <= byte.MaxValue; i++)
                Assert.Equal(i % 2 == 1, trie.Find((byte)i));
        }

        [Fact]
        public void GivenXtriePopulatedWithEvenNumbersWhenFindReturnsCorrectValue()
        {
            var trie = new XTFastTrie();
            for (int i = 0; i <= byte.MaxValue; i++)
                if (i % 2 == 0)
                    trie.Add((byte)i);
            for (int i = 0; i <= byte.MaxValue; i++)
                Assert.Equal(i % 2 == 0, trie.Find((byte)i));
        }

        [Fact]
        public void GivenEmptyXtrieWhenFindThenAlwaysReturnFalse()
        {
            var trie = new XTFastTrie();
            var rand = new Random();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                Assert.False(trie.Find(elem));
            }
        }

        [Fact]
        public void GivenTrieWhenPopulateItThenCountReturnsAmountOfAddedElements()
        {
            var trie = new XTFastTrie();
            Assert.Equal(0, trie.Count);
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                trie.Add((byte)i);
                Assert.Equal(i + 1, trie.Count);
            }
        }

        [Fact]
        public void GivenRandomCollectionWhenAddingElementsToTrieThenAddReturnsSameResultAsAddingToSet()
        {
            var trie = new XTFastTrie();
            var rand = new Random();
            var set = new HashSet<uint>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                var add = set.Add(elem);
                Assert.Equal(add, trie.Add(elem));
            }
        }
        [Fact]
        public void GivenPopulatedXtrieWhenFindThenAlwaysReturnTrue()
        {
            var trie = new XTFastTrie();
            var rand = new Random();
            var set = new HashSet<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                set.Add(elem);
                trie.Add(elem);
            }
            foreach (var elem in set)
            {
                Assert.True(trie.Find(elem));
            }
        }

        [Fact]
        public void GivenXtrieWhenAddSameNumberThenReturnFalse()
        {
            var trie = new XTFastTrie();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                Assert.True(trie.Add((byte)i));
                Assert.False(trie.Add((byte)i));
            }

            trie = new XTFastTrie();
            for (int i = byte.MaxValue; i > 0; i--)
            {
                Assert.True(trie.Add((byte)i));
                Assert.False(trie.Add((byte)i));
            }
        }

        [Fact]
        public void GivenRandomPopulatedTrieWhenTryGetNextReturnsCorrectValue()
        {
            var trie = new XTFastTrie();
            var rand = new Random();
            var list = new List<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                list.Add(elem);
                trie.Add(elem);
            }
            list.Sort();
            var arr = list.OrderBy(x => x).Distinct().ToArray();
            if (arr[0] != 0)
            {
                Assert.True(trie.TryGetNext(0, out byte res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i] - arr[i - 1] > 1)
                {
                    Assert.True(trie.TryGetNext((byte)((arr[i] - arr[i - 1]) / 2 + arr[i - 1]), out byte next));
                    Assert.Equal(arr[i], next);
                }
                Assert.True(trie.TryGetNext(arr[i - 1], out byte res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(trie.TryGetNext(arr[arr.Length - 1], out byte _));
        }

        [Fact]
        public void GivenEmptyTrieWhenTryGetNextReturnsFalse()
        {
            var trie = new XTFastTrie();
            Assert.False(trie.TryGetNext(byte.MinValue, out byte _));
        }

        [Fact]
        public void GivenTrieWithElementsLessThanSearchWhenGetNextThenReturnFalse()
        {
            var trie = new XTFastTrie();
            trie.Add(0);
            trie.Add(32);
            trie.Add(64);
            Assert.False(trie.TryGetNext(65, out byte _));
        }

        [Fact]
        public void GivenEmptyTrieWhenTryGetPrevReturnsFalse()
        {
            var trie = new XTFastTrie();
            Assert.False(trie.TryGetPrevious(byte.MaxValue, out byte _));
        }

        [Fact]
        public void GivenTrieWithElementsBiggerThanSearchWhenGetPreviousThenReturnFalse()
        {
            var trie = new XTFastTrie();
            trie.Add(128);
            trie.Add(160);
            trie.Add(196);
            Assert.False(trie.TryGetPrevious(128, out byte _));
        }

        [Fact]
        public void GivenRandomPopulatedTrieWhenGetPrevReturnsCorrectValue()
        {
            var trie = new XTFastTrie();
            var rand = new Random();
            var list = new List<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var elem = (byte)rand.Next(1 << 8);
                list.Add(elem);
                trie.Add(elem);
            }
            var arr = list.OrderByDescending(x => x).Distinct().ToArray();
            if (arr[0] != byte.MaxValue)
            {
                Assert.True(trie.TryGetPrevious(byte.MaxValue, out byte res));
                Assert.Equal(arr[0], res);
            }
            for (int i = 1; i < arr.Length - 1; i++)
            {
                if (arr[i - 1] - arr[i] > 1)
                {
                    Assert.True(trie.TryGetPrevious((byte)((arr[i - 1] - arr[i]) / 2 + arr[i]), out byte prev));
                    Assert.Equal(arr[i], prev);
                }
                Assert.True(trie.TryGetPrevious(arr[i - 1], out byte res));
                Assert.Equal(arr[i], res);
            }
            Assert.False(trie.TryGetPrevious(arr[arr.Length - 1], out byte _));
        }

        [Fact]
        public void GivenFullXtrieWhenRemoveNumberThenFindCantFindIt()
        {
            var trie = new XTFastTrie();
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                trie.Add((byte)i);
            }
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                trie.Remove((byte)i);
                Assert.False(trie.Find((byte)i));
            }
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                trie.Add((byte)i);
            }
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                var toRemove = (byte)(byte.MaxValue - i);
                trie.Remove(toRemove);
                Assert.False(trie.Find(toRemove));
            }
        }

        [Fact]
        public void GivenRandomFilledTrieWhenRemoveThenMatchesArrayState()
        {
            for (int randIter = 0; randIter < 1000; randIter++)
            {
                var trie = new XTFastTrie();
                var rand = new Random();
                var arr = new bool[1 << 8];
                for (int i = 0; i <= byte.MaxValue; i++)
                {
                    var elem = (byte)rand.Next(1 << 8);
                    arr[elem] = true;
                    trie.Add(elem);
                }

                for (int i = 0; i <= byte.MaxValue; i++)
                {
                    var elem = (byte)rand.Next(1 << 8);
                    Assert.Equal(arr[elem], trie.Remove(elem));
                    arr[elem] = false;
                }
            }
        }

        [Fact]
        public void GivenRandomFilledTtrieWhenCallMaxAndMinThenReturnProperValues()
        {
            var trie = new XTFastTrie();
            var rand = new Random();
            byte min = byte.MaxValue;
            byte max = byte.MinValue;
            for (int i = 0; i < 1000; i++)
            {
                var value = (byte)rand.Next(1 << 8);
                if (min > value) min = value;
                if (max < value) max = value;
                trie.Add(value);
            }
            Assert.Equal(min, trie.Min);
            Assert.Equal(max, trie.Max);
        }

    }
}
