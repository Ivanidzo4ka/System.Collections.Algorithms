using System.IO;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void GivenExactPowerOfTwoWhenLog2ThenReturnExactValue()
        {
            Utils.Log2(int.MaxValue - 1);
            for (var power = 0; power < 31; power++)
                Assert.Equal(power, Utils.Log2((int)Math.Pow(2, power)));
        }

        [Fact]
        public void GivenPowerOfTwoPlusOneWhenLog2ThenReturnPowerPlusOne()
        {
            for (var power = 0; power < 31; power++)
                Assert.Equal(power + 1, Utils.Log2((1 << power) + 1));
        }

        [Fact]
        public void GivenPowerOfTwoPlusMinusOneWhenLog2ThenReturnPower()
        {
            for (var power = 2; power < 31; power++)
                Assert.Equal(power, Utils.Log2((1 << power) - 1));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void GivenNumberOutsideOfScopeWhenCallingLog2ThenExplodeWithException(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Utils.Log2(value));
        }
    }
}
