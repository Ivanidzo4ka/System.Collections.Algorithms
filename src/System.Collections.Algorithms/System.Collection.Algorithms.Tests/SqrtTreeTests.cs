using System.Linq;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class SqrtTreeTests
    {
        [Fact]
        public void GivenNonEmptyEnumerationOfDataWhenCreateSqrtTreeThenNoExceptions()
        {
            Func<int, int, int> add = (a, b) => a + b;
            SqrtTree<int> tree = new SqrtTree<int>(Enumerable.Range(0, 100), add);
        }

        [Fact]
        public void GivenEmptyEnumerationOfDataWhenCreateSqrtTreeThenException()
        {
            Func<int, int, int> add = (a, b) => a + b;
            Assert.Throws<ArgumentException>(() =>
            {
                SqrtTree<int> tree = new SqrtTree<int>(Enumerable.Empty<int>(), add);
            });
        }

        [Fact]
        public void GivenTreeWhenQueryIntervalThenReturnResultOfOperationOnRange()
        {
            var size = 1024;
            var data = Enumerable.Range(1, size).ToArray();
            Func<int, int, int> add = (a, b) => a + b;
            var tree = new SqrtTree<int>(data, add);
            for (int i = 1; i < size; i++)
            {
                var res = 0;
                for (int j = i; j < size; j++)
                {
                    res = add(res, data[j]);
                    Assert.Equal(res, tree.Query(i, j));
                    Assert.Equal(res, tree.Query(j, i));
                }
            }
        }
    }
}
