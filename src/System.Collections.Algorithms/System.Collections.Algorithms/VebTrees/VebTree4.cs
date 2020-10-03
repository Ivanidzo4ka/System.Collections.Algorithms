namespace System.Collections.Algorithms
{
    internal class VebTree4
    {
        private BitArray bits;

        public VebTree4()
        {
            bits = new BitArray(16);
            Min = 16;
            Max = 0;
        }

        public bool Empty => Min == 16;

        public byte Min { get; private set; }

        public byte Max { get; private set; }

        public bool Add(byte value)
        {
            if (bits.Get(value)) return false;
            if (Min > value) Min = value;
            if (Max < value) Max = value;
            bits.Set(value, true);
            return true;
        }

        public bool Find(byte value)
        {
            return bits.Get(value);
        }

        public bool Remove(byte value)
        {
            if (!bits.Get(value)) return false;
            bits.Set(value, false);
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

        public (bool, byte) GetNext(byte x)
        {
            int pos = x + 1;
            while (pos < 16 && !bits.Get(pos))
                pos++;
            if (pos == 16)
                return (false, 0);
            else
                return (true, (byte)pos);
        }

        public (bool, byte) GetPrev(byte x)
        {
            int pos = x - 1;
            while (pos >= 0 && !bits.Get(pos))
                pos--;
            if (pos == -1)
                return (false, 16);
            else
                return (true, (byte)pos);
        }
    }
}
