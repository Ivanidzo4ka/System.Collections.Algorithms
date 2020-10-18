using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class BinaryIndexedTreeTests
    {
        [Fact]
        public void DoWork()
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
    }
}
