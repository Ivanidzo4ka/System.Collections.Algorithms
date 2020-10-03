namespace System.Collections.Algorithms
{
    /// <summary>
    /// Van Emde Boas tree for dimensionality of <see cref="uint"/>.
    /// </summary>
    public class VebTree32
    {
        private int _k;
        private VebTree32?[] _clusters;
        private VebTree32? _summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="VebTree32"/> class.
        /// </summary>
        public VebTree32()
            : this(32)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VebTree32"/> class for k dimensionality.
        /// Creates tree for K diminsionality.
        /// </summary>
        /// <param name="k">Defines dimensionality of tree. Must be power of 2.</param>
        internal VebTree32(int k)
        {
            _k = k;
            Min = 1U << k;
            if (k != 1)
            {
                _clusters = new VebTree32[1 << (k >> 1)];
            }
        }

        /// <summary>
        /// Gets minimum element in a tree.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// In case if tree empty returns <see cref="uint.MaxValue"/>.
        /// </remarks>
        public uint Min { get; private set; }

        /// <summary>
        /// Gets maximum element in a tree.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// In case if tree empty returns <see cref="uint.MinValue"/>.
        /// </remarks>
        public uint Max { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this tree empty or not.
        /// </summary>
        public bool Empty => Min == TreeLimit;

        private uint TreeLimit => 1U << _k;

        /// <summary>
        /// Adds item to <see cref="VebTree32"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 32) operation.
        /// </remarks>
        /// <param name="item">Item to add to <see cref="VebTree32"/>.</param>
        /// <returns><see langword="true"/> if item been added, and <see langword="false"/> if <see cref="VebTree32"/> already had such item.</returns>
        public bool Add(uint item)
        {
            if (Empty)
            {
                Max = item;
                Min = item;
                return true;
            }
            else if (Min == Max)
            {
                if (Min == item)
                    return false;
                if (Min < item)
                    Max = item;
                else
                    Min = item;
                return true;
            }
            else
            {
                if (Min == item || Max == item)
                    return false;
                bool added = false;
                if (Min > item)
                {
                    (Min, item) = (item, Min);
                    added = true;
                }

                if (Max < item)
                {
                    (Max, item) = (item, Max);
                    added = true;
                }

                if (_k != 1)
                {
                    var high = High(item);
                    var low = Low(item);
                    VebTree32 cluster = _clusters[high] ?? new VebTree32(_k >> 1);
                    if (cluster.Empty)
                    {
                        _summary = _summary ?? new VebTree32(_k >> 1);
                        _summary.Add(high);
                    }

                    added = cluster.Add(low);
                    _clusters[high] = cluster;
                }

                return added;
            }
        }

        /// <summary>
        /// Searches for item in <see cref="VebTree32"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 32) operation.
        /// </remarks>
        /// <param name="item">Item to search in <see cref="VebTree32"/>.</param>
        /// <returns><see langword="true"/> if item is present, and <see langword="false"/> if not present.</returns>
        public bool Find(uint item)
        {
            if (Empty)
            {
                return false;
            }

            if (Min == item || Max == item)
            {
                return true;
            }
            else
            {
                if (_k == 1)
                {
                    return false;
                }
                else
                {
                    var cluster = _clusters[High(item)];
                    if (cluster is null)
                    {
                        return false;
                    }
                    else
                    {
                        return cluster.Find(Low(item));
                    }
                }
            }
        }

        /// <summary>
        /// Trys to get next value bigger than <paramref name="value"/> in <see cref="VebTree32"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 32) operation.
        /// <paramref name="value"/> Doesn't have to be present in <see cref="VebTree32"/>.
        /// </remarks>
        /// <param name="value">Looking for item bigger than this one.</param>
        /// <param name="result">Item bigger than <paramref name="value"/>> if it exist, <see cref="uint.MaxValue"/> otherwise.</param>
        /// <returns><see langword="true"/> if such item exist, and <see langword="false"/> if not.</returns>
        public bool TryGetNext(uint value, out uint result)
        {
            var (found, ans) = GetNext(value);
            result = ans;
            return found;
        }

        /// <summary>
        /// Trys to get next value smaller than <paramref name="value"/> in <see cref="VebTree32"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 32) operation.
        /// <paramref name="value"/> Doesn't have to be present in <see cref="VebTree32"/>.
        /// </remarks>
        /// <param name="value">Looking for item smaller than this one.</param>
        /// <param name="result">Item smaller than <paramref name="value"/>> if it exist, <see cref="uint.MinValue"/> otherwise.</param>
        /// <returns><see langword="true"/> if such item exist, and <see langword="false"/> if not.</returns>
        public bool TryGetPrevious(uint value, out uint result)
        {
            var (found, ans) = GetPrev(value);
            result = ans;
            return found;
        }

        /// <summary>
        /// Remove item from <see cref="VebTree32"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 32) operation.
        /// </remarks>
        /// <param name="item">Item to remove from <see cref="VebTree32"/>.</param>
        /// <returns><see langword="true"/> if item been removed, and <see langword="false"/> if <see cref="VebTree32"/> didn't had it.</returns>
        public bool Remove(uint item)
        {
            if (Empty)
                return false;

            if (Min == item && Max == item)
            {
                Min = TreeLimit;
                Max = 0;
                return true;
            }

            if (Min == item)
            {
                if (_summary == null || _summary.Empty)
                {
                    Min = Max;
                    return true;
                }
                var minCluster = _clusters[_summary.Min];
                item = Merge(_summary.Min, minCluster!.Min);
                Min = item;
            }

            if (Max == item)
            {
                if (_summary == null || _summary.Empty)
                {
                    Max = Min;
                    return true;
                }
                else
                {
                    var maxCluster = _clusters[_summary.Max];
                    item = Merge(_summary.Max, maxCluster!.Max);
                    Max = item;
                }
            }

            if (_summary == null || _summary.Empty)
                return false;
            var high = High(item);
            var low = Low(item);
            var cluster = _clusters[high];
            if (cluster == null)
                return false;
            var removed = cluster!.Remove(low);
            if (cluster!.Empty)
            {
                _clusters[high] = default;
                _summary.Remove(high);
            }

            return removed;
        }

        private (bool, uint) GetNext(uint x)
        {
            if (Empty || Max <= x)
            {
                return (false, TreeLimit);
            }

            if (Min > x)
            {
                return (true, Min);
            }

            if (_summary == null || _summary.Empty)
            {
                return (true, Max);
            }
            else
            {
                var high = High(x);
                var low = Low(x);
                var cluster = _clusters[high];
                if (cluster != null && !cluster!.Empty && cluster!.Max > low)
                {
                    var (_, result) = cluster!.GetNext(low);
                    return (true, Merge(high, result));
                }
                else
                {
                    var (hasHigh, nextHigh) = _summary.GetNext(high);
                    if (!hasHigh)
                    {
                        return (true, Max);
                    }
                    else
                    {
                        cluster = _clusters[nextHigh];
                        return (true, Merge(nextHigh, cluster!.Min));
                    }
                }
            }
        }

        private (bool, uint) GetPrev(uint x)
        {
            if (Empty || Min >= x)
            {
                return (false, 0);
            }

            if (Max < x)
            {
                return (true, Max);
            }

            if (_summary == null || _summary.Empty)
            {
                return (true, Min);
            }
            else
            {
                var high = High(x);
                var low = Low(x);
                var cluster = _clusters[high];
                if (_clusters[high] != null && !cluster!.Empty && cluster!.Min < low)
                {
                    var (_, result) = cluster!.GetPrev(low);
                    return (true, Merge(high, result));
                }
                else
                {
                    var (hasPrev, nextPrev) = _summary.GetPrev(high);
                    if (!hasPrev)
                    {
                        return (true, Min);
                    }
                    else
                    {
                        cluster = _clusters[nextPrev];
                        return (true, Merge(nextPrev, cluster!.Max));
                    }
                }
            }
        }

        private uint High(uint x) => x >> (_k >> 1);

        private uint Low(uint x) => (uint)(x & ((1 << (_k >> 1)) - 1));

        private uint Merge(uint high, uint low) => (high << (_k >> 1)) + low;
    }
}
