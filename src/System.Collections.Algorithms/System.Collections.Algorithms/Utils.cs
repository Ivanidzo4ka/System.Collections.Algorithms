namespace System.Collections.Algorithms
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Set of useful internal utils.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Returns the integer (ceiling) log of the specified value, base 2.
        /// </summary>
        /// <param name="value">The number from which to obtain the logarithm.</param>
        /// <returns>The log of the specified value, base 2.</returns>
        public static int Log2(int value)
        {
            if (value == 0)
                return 0;
            if (value > (1 << 30))
                return 31;
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Should be in positive number.");
            int res = 0;
            while ((1 << res) < value)
            {
                res++;
            }

            return res;
        }

        /// <summary>
        /// Returns integer where all 1 in bit representation replaces with 0 except least significant bit.
        /// </summary>
        /// <param name="x">Value to apply operation.</param>
        /// <returns>Integer where all 1 in bit representation replaces with 0 except least significant bit.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IsolateLastBit(int x) => x & -x;
    }
}
