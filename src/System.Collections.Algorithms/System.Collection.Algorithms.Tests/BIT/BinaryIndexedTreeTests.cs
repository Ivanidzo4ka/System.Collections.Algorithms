using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class BinaryIndexedTreeTests
    {
        public static int Plus(int a, int b) => a + b;
        public static int Minus(int a, int b) => a - b;

        [Fact]
        public void GivenArrayWhenConstructBinaryIndexedTreeThenNoExceptions()
        {
            var tree = new BinaryIndexedTree<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.NotNull(tree);
            Assert.Equal(4, tree.Count);
        }

        [Fact]
        public void GivenNullArgumentsWhenConstructBinaryIndexedTreeThenArgumentNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new BinaryIndexedTree<int>(null, null, null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new BinaryIndexedTree<int>(new[] { 1, 2, 3, 4 }, null, null);
            });

            Assert.Throws<NotSupportedException>(() =>
            {
                var tree = new BinaryIndexedTree<int>(new[] { 1, 2, 3, 4 }, (a, b) => (a | b), null);
                tree[1] = 3;

            });

            Assert.Throws<ArgumentException>(() =>
            {
                var tree = new BinaryIndexedTree<int>(new int[0] { }, (a, b) => (a | b), (a, b) => (a | b));
            });
        }

        [Fact]
        public void GivenSumBinaryIndexedTreeWhenAskOperationOnIntervalThenReturnCorrectValues()
        {
            var data = Enumerable.Range(1, 200).ToArray();
            var tree = new BinaryIndexedTree<int, int>(data, (a, b) => a + b, (a, b) => a - b, (x) => x);
            for (int i = 0; i < data.Length; i++)
            {
                int sum = 0;
                for (int j = i; j < data.Length; j++)
                {
                    sum += data[j];
                    Assert.Equal(sum, tree.GetOperationValueOnInterval(i, j));
                }
            }
        }

        [Fact]
        public void GivenSumBinaryIndexedTreeWhenUpdateChangeValueThenIntervalUpdated()
        {
            var data = Enumerable.Range(1, 200).ToArray();
            var tree = new BinaryIndexedTree<int, int>(data, (a, b) => a + b, (a, b) => a - b, (x) => x);
            data[10] = 100;
            tree[10] = 100;
            for (int i = 0; i < data.Length; i++)
            {
                int sum = 0;
                for (int j = i; j < data.Length; j++)
                {
                    sum += data[j];
                    Assert.Equal(sum, tree.GetOperationValueOnInterval(i, j));
                }
            }
        }

        [Fact]
        public void GivenMinBinaryIndexedTreeWhenUpdateChangeValueThenIntervalUpdated()
        {
            var data = Enumerable.Range(1, 200).ToArray();
            var tree = new BinaryIndexedTree<int>(data,
                (a, b) => Math.Min(a, b),
                (a, b) =>
                {
                    if (a > b) throw new Exception("Can only decrease value");
                    return Math.Min(a, b);
                },
                int.MaxValue
                );
            for (int i = 0; i < data.Length; i++)
            {
                int min = int.MaxValue;
                for (int j = i; j < data.Length; j++)
                {
                    min = Math.Min(min, data[j]);
                    Assert.Equal(min, tree.GetOperationValueOnInterval(i, j));
                }
            }
        }

        [Fact]
        public void GivenSumBinaryIndexedTreeWhenFilledWithRandomThenIntervalReturnsCorrectValues()
        {
            var rand = new Random();
            var data = new uint[64];
            for (int i = 0; i < data.Length; i++)
                data[i] = (uint)rand.Next();
            var tree = new BinaryIndexedTree<uint>(data, (a, b) => a + b, (a, b) => a - b);
            for (int i = 0; i < data.Length; i++)
            {
                uint sum = 0;
                for (int j = i; j < data.Length; j++)
                {
                    sum += data[j];
                    Assert.Equal(sum, tree.GetOperationValueOnInterval(i, j));
                }
            }
        }

        [Fact]
        public void GivenRandomSumBinaryIndexedTreeWhenUpdateValuesThenIntervalReturnsCorrectValues()
        {
            var rand = new Random();
            var data = new uint[64];
            for (int i = 0; i < data.Length; i++)
                data[i] = (uint)rand.Next();
            var tree = new BinaryIndexedTree<uint>(data, (a, b) => a + b, (a, b) => a - b);
            for (int i = 0; i < data.Length; i++)
            {
                var elem = (uint)rand.Next();
                data[i] = elem;
                tree[i] = elem;
            }

            for (int i = 0; i < data.Length; i++)
            {
                uint sum = 0;
                for (int j = i; j < data.Length; j++)
                {
                    sum += data[j];
                    Assert.Equal(sum, tree.GetOperationValueOnInterval(i, j));
                }
            }
        }

        [Fact]
        public void GivenRandomSumBinaryIndexedTreeWhenEnumerateThenSameAsOriginalCollection()
        {
            var rand = new Random(1);
            var data = new uint[64];
            for (int i = 0; i < data.Length; i++)
                data[i] = (uint)rand.Next();
            var tree = new BinaryIndexedTree<uint>(data, (a, b) => a + b, (a, b) => a - b);
            Assert.Equal(data, tree);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (uint)rand.Next();
                tree[i] = data[i];
                Assert.Equal(data[i], tree[i]);
            }
            Assert.Equal(data, tree);
        }

        [Fact]
        public void GivenBinaryIndexedTreeWhenSetOrGetThroughIndexerOutsideOfArrayThenThrowException()
        {
            var tree = new BinaryIndexedTree<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[4]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[-1] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[4] = 1);
        }

        [Fact]
        public void GivenBinaryIndexedTreeWithSelectorWhenCreateTreeThenNoException()
        {
            var tree = new BinaryIndexedTree<Stub, int>(new[] { new Stub() { A = 1, B = 1 }, new Stub() { A = 100, B = 2 } }, Plus, Minus, (x) => x.B);
            Assert.Equal(3, tree.GetOperationValueOnInterval(0, 1));
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(2, 1)]
        [InlineData(2, 4)]
        [InlineData(4, 2)]
        public void GivenBinaryIndexedTreeWheTryIntervalOutsideOfRangeThenThrowException(int left, int right)
        {
            var tree = new BinaryIndexedTree<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.Throws<ArgumentOutOfRangeException>(() =>tree.GetOperationValueOnInterval(left, right));
        }

        private class Stub
        {
            public int A;
            public int B;
        }
    }
}
