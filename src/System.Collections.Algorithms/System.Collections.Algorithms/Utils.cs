namespace System.Collections.Algorithms
{
    /// <summary>
    /// Set of useful internal utils.
    /// </summary>
    internal static class Utils
    {
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
    }
}
