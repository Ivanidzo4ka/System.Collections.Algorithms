using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class SqrtTreeTests
    {
        private static int Sum(int a, int b) => a + b;

        [Fact]
        public void GivenNonEmptyEnumerationOfDataWhenCreateSqrtTreeThenNoExceptions()
        {
            SqrtTree<int> tree = new SqrtTree<int>(Enumerable.Range(0, 100), Sum);
        }

        [Fact]
        public void GivenLabmdaWhenCreateSqrtTreeThenNoExceptions()
        {
            Func<int, int, int> add = (a, b) => a + b;
            SqrtTree<int> tree = new SqrtTree<int>(Enumerable.Range(0, 100), add);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData((1 << 30) + 1)]
        public void GivenUnsupportedCapacityWhenCreateSqrtTreeThenException(int capacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                SqrtTree<int> tree = new SqrtTree<int>(capacity, Sum);
            });
        }
        [Fact]
        public void GivenCapacityWhenCreateSqrtTreeThenNoExceptions()
        {
            SqrtTree<int> tree = new SqrtTree<int>(100, Sum);
        }

        [Fact]
        public void GivenEmptyEnumerationOfDataWhenCreateSqrtTreeThenException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SqrtTree<int> tree = new SqrtTree<int>(Enumerable.Empty<int>(), Sum);
            });
        }

        [Fact]
        public void GivenNullOperationWhenCreateSqrtTreeThenException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                SqrtTree<int> tree = new SqrtTree<int>(Enumerable.Range(1, 3), null);
            });
        }


        [Fact]
        public void GivenNullAsEnumerableWhenCreateSqrtTreeThenException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                SqrtTree<int> tree = new SqrtTree<int>(null, Sum);
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(128)]
        public void GivenTreeWhenQueryIntervalThenReturnResultOfOperationOnRange(int size)
        {
            var data = Enumerable.Range(1, size).ToArray();
            var tree = new SqrtTree<int>(data, Sum);
            for (int i = 0; i < size; i++)
            {
                var res = 0;
                for (int j = i; j < size; j++)
                {
                    res = Sum(res, data[j]);
                    Assert.Equal(res, tree.Query(i, j));
                    Assert.Equal(res, tree.Query(j, i));
                }
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public void GivenTreeWhenAccessIndexOutsizeOfTreeSizeThenThrowException(int index)
        {
            var tree = new SqrtTree<int>(10, Sum);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[index]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public void GivenTreeWhenUpdateIndexOutsizeOfTreeSizeThenThrowException(int index)
        {
            var tree = new SqrtTree<int>(10, Sum);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree[index] = 100);
        }

        [Fact]
        public void GivenArrayAndConstructTreeWhenChangeArrayThenResultsInTreeAreNotChanged()
        {
            var data = new int[4] { 1, 2, 3, 4 };
            var tree = new SqrtTree<int>(data, Sum);
            var sum = tree.Query(0, 3);
            data[1] = 10;
            tree[2] = 10;
            tree[2] = 3;
            Assert.Equal(sum, tree.Query(0, 3));
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(100, 0)]
        [InlineData(-1, 99)]
        [InlineData(99, -1)]
        [InlineData(-1, 100)]
        public void GivenTreeWhenQueueOutsideOfRangeThenThrowException(int left, int right)
        {
            var tree = new SqrtTree<int>(100, Sum);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tree.Query(left, right);
            });
        }

        [Fact]
        public void GivenTreeWhenUpdateElementThenQueuedReflectsChanges()
        {
            int size = 1_000_000;
            Func<int, int, int> add = (a, b) => a + b;
            var updatedTree = new SqrtTree<int>(Enumerable.Range(1, size), add);
            var canonicalTree = new SqrtTree<int>(Enumerable.Range(1, size), add);
            updatedTree[size / 2] = updatedTree[size / 2] + 100;
            for (int i = 0; i < size; i++)
                for (int j = i; j < size; j++)
                    Assert.Equal(canonicalTree.Query(i, j), updatedTree.Query(i, j)
                        - ((i <= size / 2 && j >= size / 2) ? 100 : 0));
        }

        [Fact]
        public void GivenCapacityWhenUpdateThenQueueReflectChanges()
        {
            var tree = new SqrtTree<int>(10, Sum);
            tree[1] = 10;
            tree[5] = 20;
            Assert.Equal(30, tree.Query(0, 9));
            Assert.Equal(10, tree.Query(1, 1));
            Assert.Equal(20, tree.Query(5, 5));
        }
    }
}
