namespace System.Collections.Algorithms
{
    public class VebTree32
    {
        private int _k;
        uint _min;
        uint _max;
        private VebTree32[] _clusters;
        private VebTree32 _summary;

        public VebTree32(int k)
        {
            _k = k;
            _min = 1U << k;
            if (k != 1)
            {
                _clusters = new VebTree32[1 << (k >> 1)];
            }
        }

        public void Insert(uint x)
        {
            if (Empty)
            {
                _max = x;
                _min = x;
            }

            if (x < _min)
            {
                (x, _min) = (_min, x);
            }

            if (x > _max)
                _max = x;
            if (_k != 1)
            {
                var high = High(x);
                var low = Low(x);
                if (_clusters[high] == null)
                {
                    _clusters[high] = new VebTree32(_k >> 1);

                }
                if (_clusters[high].Empty)
                {
                    if (_summary == null)
                    {
                        _summary = new VebTree32(_k >> 1);
                    }

                    _summary.Insert(high);
                }
                _clusters[high].Insert(low);
            }
        }

        public bool Find(uint x)
        {
            if (Empty)
                return false;
            if (_min == x || _max == x)
                return true;
            else
            {
                if (_k == 1)
                    return false;
                else
                {
                    return _clusters[High(x)] != null && _clusters[High(x)].Find(Low(x));
                }
            }
        }

        public uint GetNext(uint x)
        {
            if (_min <= x)
                return _min;
            if (Empty || x > _max)
                return 1U << _k;
            if (_k == 1)
            {
                return _max == x ? x : 1U << _k;
            }

            var high = High(x);
            var low = Low(x);
            if (_clusters[high] != null && low <= _clusters[high]._max)
            {
                return Merge(high, _clusters[high].GetNext(low));
            }
            else if (_summary != null)
            {
                var nextHigh = _summary.GetNext(high + 1);
                if (nextHigh != (1U << (_k >> 1)))
                    return Merge(nextHigh, _clusters[nextHigh]._min);

            }

            return 1U << _k;
        }

        public void Remove(uint x)
        {
            if (_min == TreeLimit)
                return;
            if (x < _min || x > _max)
                return;
            if (_min == x && _max == x)
            {
                _min = 1U << _k;
                _max = 0;
                return;
            }
            else
            {
                var high = High(x);
                var low = Low(x);
                if (_min == x)
                {
                    if (_summary != null && _summary._min != (1U << (_k >> 1)))
                    {
                        var y = _clusters[_summary._min]._min;
                        _min = y;
                    }
                }
                if (_clusters[high] != null)
                {
                    _clusters[high].Remove(low);
                    if (_clusters[high]._min == TreeLimit)
                        _summary.Remove(high);
                    if (x == _max)
                    {
                        var y = _summary._max;
                        if (_summary.Empty)
                            _max = _min;
                        else
                            _max = Merge(y, _clusters[y]._max);
                    }
                }
                else if (x == _max)
                { _max = Merge(high, _clusters[high]._max); }

            }
        }

        private uint High(uint x) => x >> (_k >> 1);

        private uint Low(uint x) => (uint)(x & ((1 << (_k >> 1)) - 1));

        private uint Merge(uint high, uint low) => (high << (_k >> 1)) + low;

        private bool Empty => _min == TreeLimit;

        private uint TreeLimit => 1U << _k;
    }
}
