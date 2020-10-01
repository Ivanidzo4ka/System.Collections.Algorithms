using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class VebTree32Tests
    {
        [Fact]
        public void GivenVebTreePopulatedWhenFindReturnsCorrectValue()
        {
            var tree = new VebTree32(8);
            tree.Insert(1);
            tree.Insert(10);
            tree.Insert(9);
            tree.Insert(5);
            tree.Insert(100);
            var t = tree.Find(101);
            t = tree.Find(100);
            t = tree.Find(0);
            t = tree.Find(1);
            t = tree.Find(2);
        }
    }
}
