using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class FenwickTreeTests
    {
        public static int Plus(int a, int b) => a + b;
        public static int Minus(int a, int b) => a - b;

        [Fact]
        public void GivenArrayWhenConstructFenwickTreeThenNoExceptions()
        {
            var tree = new FenwickTree<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.NotNull(tree);
            Assert.Equal(4, tree.Count);
        }

        [Fact]
        public void GivenNullArgumentsWhenConstructFenwickTreeThenArgumentNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new FenwickTree<int>(null, null, null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new FenwickTree<int>(new[] { 1, 2, 3, 4 }, null, null);
            });

            Assert.Throws<NotSupportedException>(() =>
            {
                var tree = new FenwickTree<int>(new[] { 1, 2, 3, 4 }, (a, b) => (a | b), null);
                tree[3] = 3;

            });

            Assert.Throws<ArgumentException>(() =>
            {
                var tree = new FenwickTree<int>(new int[0] { }, (a, b) => (a | b), (a, b) => (a | b));
            });
        }

        [Fact]
        public void GivenSumFenwickTreeWhenAskOperationOnIntervalThenReturnCorrectValues()
        {
            var tree = new FenwickTree<int>(Enumerable.Range(1, 100), Plus, Minus);
            Assert.Equal(1, tree.GetOperationValueOnInterval(0));
            for (int i = 1; i < tree.Count; i++)
                Assert.Equal((i + 2) * (i + 1) / 2, tree.GetOperationValueOnInterval(i));
        }

        [Fact]
        public void GivenSumFenwickTreeWhenUpdateChangeValueThenIntervalUpdated()
        {
            var tree = new FenwickTree<int>(Enumerable.Range(1, 5), Plus, Minus);
            tree[0] = 10;
            Assert.Equal(24, tree.GetOperationValueOnInterval(4));
            tree[4] = 10;
            Assert.Equal(29, tree.GetOperationValueOnInterval(4));
        }

        [Fact]
        public void GivenMinFenwickTreeWhenUpdateChangeValueThenIntervalUpdated()
        {
            var tree = new FenwickTree<int>(Enumerable.Range(100, 5),
                (a, b) => Math.Min(a, b),
                (a, b) =>
                {
                    if (a > b) throw new Exception("Can only decrease value");
                    return Math.Min(a, b);
                },
                int.MaxValue
                );
            Assert.Equal(100, tree.GetOperationValueOnInterval(4));
            tree[3] = 10;
            Assert.Equal(100, tree.GetOperationValueOnInterval(2));
            Assert.Equal(10, tree.GetOperationValueOnInterval(3));
        }

        [Fact]
        public void GivenSumFenwickTreeWhenFilledWithRandomThenIntervalReturnsCorrectValues()
        {
            var rand = new Random();
            var arr = new uint[64];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (uint)rand.Next();
            var tree = new FenwickTree<uint>(arr, (a, b) => a + b, (a, b) => a - b);
            uint sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum += arr[i];
                Assert.Equal(sum, tree.GetOperationValueOnInterval(i));
            }
        }

        [Fact]
        public void GivenRandomSumFenwickTreeWhenUpdateValuesThenIntervalReturnsCorrectValues()
        {
            var rand = new Random();
            var arr = new uint[64];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (uint)rand.Next();
            var tree = new FenwickTree<uint>(arr, (a, b) => a + b, (a, b) => a - b);
            uint sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                var elem = (uint)rand.Next();
                arr[i] = elem;
                tree[i] = elem;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                sum += arr[i];
                Assert.Equal(sum, tree.GetOperationValueOnInterval(i));
            }
        }

        [Fact]
        public void GivenRandomSumFenwickTreeWhenEnumerateThenSameAsOriginalCollection()
        {
            var rand = new Random();
            var arr = new uint[64];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (uint)rand.Next();
            var tree = new FenwickTree<uint>(arr, (a, b) => a + b, (a, b) => a - b);
            Assert.Equal(arr, tree);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (uint)rand.Next();
                tree[i] = arr[i];
                Assert.Equal(arr[i], tree[i]);
            }
            Assert.Equal(arr, tree);
        }

        [Fact]
        public void GivenFenwickTreeWhenSetOrGetThroughIndexerOutsideOfArrayThenThrowException()
        {
            var tree = new FenwickTree<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[4]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[-1] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[4] = 1);
        }

        [Fact]
        public void GivenSumFenwickTreeWithSelectorWhenCreateTreeThenNoException()
        {
            var tree = new FenwickTree<Stub, int>(new[] { new Stub() { A = 1, B = 1 }, new Stub() { A = 100, B = 2 } }, Plus, Minus, (x) => x.B);
            Assert.Equal(3, tree.GetOperationValueOnInterval(1));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void GivenBinaryIndexedTreeWheTryIntervalOutsideOfRangeThenThrowException(int pos)
        {
            var tree = new FenwickTree<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.GetOperationValueOnInterval(pos));
        }

        private class Stub
        {
            public int A;
            public int B;
        }
    }
}
