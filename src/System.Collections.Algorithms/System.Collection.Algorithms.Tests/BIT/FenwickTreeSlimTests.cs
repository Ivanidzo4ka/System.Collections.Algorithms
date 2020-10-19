using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests.BIT
{
    public class FenwickTreeSlimSlimTests
    {
        public static int Plus(int a, int b) => a + b;
        public static int Minus(int a, int b) => a - b;

        [Fact]
        public void GivenArrayWhenConstructFenwickTreeSlimThenNoExceptions()
        {
            var tree = new FenwickTreeSlim<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.NotNull(tree);
            Assert.Equal(4, tree.Count);
        }

        [Fact]
        public void GivenNullArgumentsWhenConstructFenwickTreeSlimThenArgumentNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new FenwickTreeSlim<int>(null, null, null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new FenwickTreeSlim<int>(new[] { 1, 2, 3, 4 }, null, null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var tree = new FenwickTreeSlim<int>(new[] { 1, 2, 3, 4 }, (a, b) => (a | b), null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var tree = new FenwickTreeSlim<int>(new int[0] { }, (a, b) => (a | b), (a, b) => (a | b));
            });
        }

        [Fact]
        public void GivenSumFenwickTreeSlimWhenAskOperationOnIntervalThenReturnCorrectValues()
        {
            var tree = new FenwickTreeSlim<int>(Enumerable.Range(1, 100), Plus, Minus);
            Assert.Equal(1, tree.GetOperationValueOnInterval(0));
            for (int i = 1; i < tree.Count; i++)
                Assert.Equal((i + 2) * (i + 1) / 2, tree.GetOperationValueOnInterval(i));
        }

        [Fact]
        public void GivenSumFenwickTreeSlimWhenUpdateChangeValueThenIntervalUpdated()
        {
            var tree = new FenwickTreeSlim<int>(Enumerable.Range(1, 5), Plus, Minus);
            tree[0] = 10;
            Assert.Equal(24, tree.GetOperationValueOnInterval(4));
            tree[4] = 10;
            Assert.Equal(29, tree.GetOperationValueOnInterval(4));
        }

        [Fact]
        public void GivenMinFenwickTreeSlimWhenUpdateChangeValueThenIntervalUpdated()
        {
            var tree = new FenwickTreeSlim<int>(Enumerable.Range(100, 5),
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
        public void GivenSumFenwickTreeSlimWhenFilledWithRandomThenIntervalReturnsCorrectValues()
        {
            var rand = new Random();
            var arr = new uint[257];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (uint)rand.Next();
            var tree = new FenwickTreeSlim<uint>(arr, (a, b) => a + b, (a, b) => a - b);
            uint sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum += arr[i];
                Assert.Equal(sum, tree.GetOperationValueOnInterval(i));
            }
        }

        [Fact]
        public void GivenRandomSumFenwickTreeSlimWhenUpdateValuesThenIntervalReturnsCorrectValues()
        {
            var rand = new Random();
            var arr = new uint[64];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (uint)rand.Next();
            var tree = new FenwickTreeSlim<uint>(arr, (a, b) => a + b, (a, b) => a - b);
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
        public void GivenRandomSumFenwickTreeSlimWhenEnumerateThenSameAsOriginalCollection()
        {
            var rand = new Random();
            var arr = new uint[4];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (uint)rand.Next();
            var tree = new FenwickTreeSlim<uint>(arr, (a, b) => a + b, (a, b) => a - b);
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
        public void GivenFenwickTreeSlimWhenSetOrGetThroughIndexerOutsideOfArrayThenThrowException()
        {
            var tree = new FenwickTreeSlim<int>(new[] { 1, 2, 3, 4 }, Plus, Minus);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[4]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[-1] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[4] = 1);
        }

    }
}
