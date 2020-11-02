using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Collections.Algorithms.Tests.BIT
{
    public class CounterFenwickTreeTests
    {
        public static int Plus(int a, int b) => a + b;
        public static int Minus(int a, int b) => a - b;

        [Fact]
        public void GivenArrayWhenConstructFenwickTreeSlimThenNoExceptions()
        {
            var tree = new CounterFenwickTree<int>(Enumerable.Range(1,8), Plus, Minus);
            tree.Ask(1, 6);
            Assert.NotNull(tree);
            Assert.Equal(4, tree.Count);
        }
    }
}
