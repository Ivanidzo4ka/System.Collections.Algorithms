namespace System.Collections.Algorithms
{
    /// <summary>
    /// Internal class to represent Van Emdbe Boas tree for dimension of 4 bits.
    /// </summary>
    /// <remarks>
    /// Theoretically it should keep inside itself trees of dimensions of 2 bits, but i don't see much sense for that.
    /// <see cref="BitArray"/> could to the trick to preserve information and be fast enough, althrough maybe refactoring into bit operations with
    /// fast highest bit and lowest bit would bring better performance.
    /// </remarks>
    internal class VaneEmdeBoasTree4
    {
        private BitArray bits;

        /// <summary>
        /// Initializes a new instance of the <see cref="VaneEmdeBoasTree4"/> class.
        /// </summary>
        public VaneEmdeBoasTree4()
        {
            bits = new BitArray(16);
            Min = 16;
            Max = 0;
        }

        /// <summary>
        /// Gets a value indicating whether this tree empty or not.
        /// </summary>
        public bool Empty => Min == 16;

        /// <summary>
        /// Gets minimum element in a tree.
        /// </summary>
        public byte Min { get; private set; }

        /// <summary>
        /// Gets maximum element in a tree.
        /// </summary>
        public byte Max { get; private set; }

        /// <summary>
        /// Adds item to <see cref="VaneEmdeBoasTree4"/>.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// </remarks>
        /// <param name="item">Item to add to <see cref="VaneEmdeBoasTree4"/>.</param>
        /// <returns><see langword="true"/> if item been added, and <see langword="false"/> if <see cref="VaneEmdeBoasTree4"/> already had such item.</returns>
        public bool Add(byte item)
        {
            if (bits.Get(item)) return false;
            if (Min > item) Min = item;
            if (Max < item) Max = item;
            bits.Set(item, true);
            return true;
        }

        /// <summary>
        /// Searches for item in <see cref="VanEmdeBoasTree4"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 8) operation.
        /// </remarks>
        /// <param name="item">Item to search in <see cref="VanEmdeBoasTree4"/>.</param>
        /// <returns><see langword="true"/> if item is present, and <see langword="false"/> if not present.</returns>
        public bool Find(byte item)
        {
            return bits.Get(item);
        }

        /// <summary>
        /// Remove item from <see cref="VanEmdeBoasTree4"/>.
        /// </summary>
        /// <param name="item">Item to remove from <see cref="VanEmdeBoasTree4"/>.</param>
        /// <returns><see langword="true"/> if item been removed, and <see langword="false"/> if <see cref="VanEmdeBoasTree4"/> didn't had it.</returns>
        public bool Remove(byte item)
        {
            if (!bits.Get(item)) return false;
            bits.Set(item, false);
            var pos = Min;
            while (pos < 16 && !bits.Get(pos))
                pos++;
            if (pos == 16)
            {
                Min = 16;
                Max = 0;
                return true;
            }

            var maxPos = Max;
            while (maxPos > pos && !bits.Get(maxPos))
                maxPos--;
            Max = maxPos;
            Min = pos;
            return true;
        }

        /// <summary>
        /// Return first element in <see cref="VanEmdeBoasTree4"/> bigger than <paramref name="threshold"/>.
        /// </summary>
        /// <param name="threshold">Threshold value.</param>
        /// <returns>Tuple where first part is next element exist, and second part is founded element or <see cref="ulong.MaxValue"/>.</returns>
        public (bool, byte) GetNext(byte threshold)
        {
            int pos = threshold + 1;
            while (pos < 16 && !bits.Get(pos))
                pos++;
            if (pos == 16)
                return (false, 0);
            else
                return (true, (byte)pos);
        }

        /// <summary>
        /// Return last element in <see cref="VanEmdeBoasTree4"/> smaller than <paramref name="threshold"/>.
        /// </summary>
        /// <param name="threshold">Threshold value.</param>
        /// <returns>Tuple where first part is next element exist, and second part is founded element or <see cref="ulong.MinValue"/>.</returns>
        public (bool, byte) GetPrev(byte threshold)
        {
            if (threshold == 0) return (false, 16);
            int pos = threshold - 1;
            while (pos >= 0 && !bits.Get(pos))
                pos--;
            if (pos == -1)
                return (false, 16);
            else
                return (true, (byte)pos);
        }
    }
}
