namespace System.Collections.Algorithms
{
    public class VebTree32
    {
        private int _k;
        private uint _min;
        private uint _max;
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

        public uint Min => _min;

        public uint Max => _max;

        public bool Empty => _min == TreeLimit;

        public uint TreeLimit => 1U << _k;

        public bool Add(uint x)
        {
            if (Empty)
            {
                _max = x;
                _min = x;
                return true;
            }
            else if (_min == _max)
            {
                if (_min == x)
                    return false;
                if (_min < x)
                    _max = x;
                else
                    _min = x;
                return true;
            }
            else
            {
                if (_min == x || _max == x)
                    return false;
                bool added = false;
                if (_min > x)
                {
                    (_min, x) = (x, _min);
                    added = true;
                }
                if (_max < x)
                {
                    (_max, x) = (x, _max);
                    added = true;
                }

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
                        _summary = _summary ?? new VebTree32(_k >> 1);
                        _summary.Add(high);
                    }

                    return _clusters[high].Add(low);
                }
                return added;
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

        public bool TryGetNext(uint x, out uint result)
        {
            var (found, ans) = GetNext(x);
            result = ans;
            return found;
        }

        public bool TryGetPrev(uint x, out uint result)
        {
            var (found, ans) = GetPrev(x);
            result = ans;
            return found;
        }

        public bool Remove(uint x)
        {
            if (_min == x && _max == x)
            {
                _min = TreeLimit;
                _max = 0;
                return true;
            }

            if (_min == x)
            {
                if (_summary == null || _summary.Empty)
                {
                    _min = _max;
                    return true;
                }

                x = Merge(_summary._min, _clusters[_summary._min]._min);
                _min = x;
            }

            if (_max == x)
            {
                if (_summary == null || _summary.Empty)
                {
                    _max = _min;
                    return true;
                }
                else
                {
                    x = Merge(_summary._max, _clusters[_summary._max]._max);
                    _max = x;
                }
            }

            if (_summary == null || _summary.Empty)
                return false;
            var high = High(x);
            var low = Low(x);
            var removed = _clusters[high].Remove(low);
            if (_clusters[high].Empty)
            {
                _summary.Remove(high);
                _clusters[high] = default;
            }
            return removed;
        }

        private (bool, uint) GetNext(uint x)
        {
            if (Empty || _max <= x)
            {
                return (false, TreeLimit);
            }

            if (_min > x)
            {
                return (true, _min);
            }

            if (_summary == null || _summary.Empty)
            {
                return (true, _max);
            }
            else
            {

                var high = High(x);
                var low = Low(x);
                if (_clusters[high] != null && !_clusters[high].Empty && _clusters[high]._max > low)
                {
                    var (_, result) = _clusters[high].GetNext(low);
                    return (true, Merge(high, result));
                }
                else
                {
                    var (hasHigh, nextHigh) = _summary.GetNext(high);
                    if (!hasHigh)
                        return (true, _max);
                    else
                        return (true, Merge(nextHigh, _clusters[nextHigh]._min));
                }
            }
        }

        private (bool, uint) GetPrev(uint x)
        {
            if (Empty || _min >= x)
            {
                return (false, 0);
            }

            if (_max < x)
            {
                return (true, _max);
            }

            if (_summary == null || _summary.Empty)
            {
                return (true, _min);
            }
            else
            {

                var high = High(x);
                var low = Low(x);
                if (_clusters[high] != null && !_clusters[high].Empty && _clusters[high]._min < low)
                {
                    var (_, result) = _clusters[high].GetPrev(low);
                    return (true, Merge(high, result));
                }
                else
                {
                    var (hasHigh, nextHigh) = _summary.GetPrev(high);
                    if (!hasHigh)
                        return (true, _min);
                    else
                        return (true, Merge(nextHigh, _clusters[nextHigh]._max));
                }
            }
        }

        private uint High(uint x) => x >> (_k >> 1);

        private uint Low(uint x) => (uint)(x & ((1 << (_k >> 1)) - 1));

        private uint Merge(uint high, uint low) => (high << (_k >> 1)) + low;

    }
}
