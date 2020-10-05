namespace System.Collections.Algorithms
{
    using System.Collections.Generic;

    /// <summary>
    /// Van Emde Boas tree for dimensionality of <see cref="ulong"/>.
    /// </summary>
    public class VanEmdeBoasTree64
    {
        private Dictionary<uint, VanEmdeBoasTree32?> _clusters;
        private VanEmdeBoasTree32? _summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="VanEmdeBoasTree64"/> class.
        /// </summary>
        public VanEmdeBoasTree64()
        {
            Min = ulong.MaxValue;
            Count = 0;
            _clusters = new Dictionary<uint, VanEmdeBoasTree32?>();
        }

        /// <summary>
        /// Gets minimum element in a tree.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// In case if tree empty returns <see cref="ulong.MaxValue"/>.
        /// </remarks>
        public ulong Min { get; private set; }

        /// <summary>
        /// Gets maximum element in a tree.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// In case if tree empty returns <see cref="ulong.MinValue"/>.
        /// </remarks>
        public ulong Max { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this tree empty or not.
        /// </summary>
        public bool Empty => Count == 0;

        /// <summary>
        /// Gets the number of items that are contained in a <see cref="VanEmdeBoasTree64"/>.
        /// </summary>
        public ulong Count { get; private set; }

        /// <summary>
        /// Adds item to <see cref="VanEmdeBoasTree64"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 64) operation.
        /// </remarks>
        /// <param name="item">Item to add to <see cref="VanEmdeBoasTree64"/>.</param>
        /// <returns><see langword="true"/> if item been added, and <see langword="false"/> if <see cref="VanEmdeBoasTree64"/> already had such item.</returns>
        public bool Add(ulong item)
        {
            if (Empty)
            {
                Max = item;
                Min = item;
                Count++;
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
                Count++;
                return true;
            }
            else
            {
                if (Min == item || Max == item)
                    return false;
                if (Min > item)
                {
                    (Min, item) = (item, Min);
                }

                if (Max < item)
                {
                    (Max, item) = (item, Max);
                }

                var high = High(item);
                var low = Low(item);
                _clusters.TryGetValue(high, out VanEmdeBoasTree32? cluster);
                cluster = cluster ?? new VanEmdeBoasTree32();
                if (cluster.Empty)
                {
                    _summary = _summary ?? new VanEmdeBoasTree32();
                    _summary.Add(high);
                }

                var added = cluster.Add(low);
                _clusters[high] = cluster;
                if (added)
                    Count++;
                return added;
            }
        }

        /// <summary>
        /// Searches for item in <see cref="VanEmdeBoasTree64"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 64) operation.
        /// </remarks>
        /// <param name="item">Item to search in <see cref="VanEmdeBoasTree64"/>.</param>
        /// <returns><see langword="true"/> if item is present, and <see langword="false"/> if not present.</returns>
        public bool Find(ulong item)
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
                _clusters.TryGetValue(High(item), out VanEmdeBoasTree32? cluster);
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

        /// <summary>
        /// Trys to get next value bigger than <paramref name="value"/> in <see cref="VanEmdeBoasTree64"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 64) operation.
        /// <paramref name="value"/> Doesn't have to be present in <see cref="VanEmdeBoasTree64"/>.
        /// </remarks>
        /// <param name="value">Looking for item bigger than this one.</param>
        /// <param name="result">Item bigger than <paramref name="value"/>> if it exist, <see cref="ulong.MaxValue"/> otherwise.</param>
        /// <returns><see langword="true"/> if such item exist, and <see langword="false"/> if not.</returns>
        public bool TryGetNext(ulong value, out ulong result)
        {
            var (found, ans) = GetNext(value);
            result = ans;
            return found;
        }

        /// <summary>
        /// Trys to get next value smaller than <paramref name="value"/> in <see cref="VanEmdeBoasTree64"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 64) operation.
        /// <paramref name="value"/> Doesn't have to be present in <see cref="VanEmdeBoasTree64"/>.
        /// </remarks>
        /// <param name="value">Looking for item smaller than this one.</param>
        /// <param name="result">Item smaller than <paramref name="value"/>> if it exist, <see cref="ulong.MinValue"/> otherwise.</param>
        /// <returns><see langword="true"/> if such item exist, and <see langword="false"/> if not.</returns>
        public bool TryGetPrevious(ulong value, out ulong result)
        {
            var (found, ans) = GetPrev(value);
            result = ans;
            return found;
        }

        /// <summary>
        /// Remove item from <see cref="VanEmdeBoasTree64"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 64) operation.
        /// </remarks>
        /// <param name="item">Item to remove from <see cref="VanEmdeBoasTree64"/>.</param>
        /// <returns><see langword="true"/> if item been removed, and <see langword="false"/> if <see cref="VanEmdeBoasTree64"/> didn't had it.</returns>
        public bool Remove(ulong item)
        {
            if (Empty)
                return false;

            if (Min == item && Max == item)
            {
                Min = ulong.MaxValue;
                Max = ulong.MinValue;
                Count--;
                return true;
            }

            if (Min == item)
            {
                if (_summary == null || _summary.Empty)
                {
                    Min = Max;
                    Count--;
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
                    Count--;
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
            _clusters.TryGetValue(high, out VanEmdeBoasTree32? cluster);
            if (cluster == null)
                return false;
            var removed = cluster!.Remove(low);
            if (cluster!.Empty)
            {
                _clusters.Remove(high);
                _summary.Remove(high);
            }

            if (removed)
                Count--;
            return removed;
        }

        /// <summary>
        /// Return first element in <see cref="VanEmdeBoasTree64"/> bigger than <paramref name="threshold"/>.
        /// </summary>
        /// <param name="threshold">Threshold value.</param>
        /// <returns>Tuple where first part is next element exist, and second part is founded element or <see cref="ulong.MaxValue"/>.</returns>
        internal (bool, ulong) GetNext(ulong threshold)
        {
            if (Empty || Max <= threshold)
            {
                return (false, ulong.MaxValue);
            }

            if (Min > threshold)
            {
                return (true, Min);
            }

            if (_summary == null || _summary.Empty)
            {
                return (true, Max);
            }
            else
            {
                var high = High(threshold);
                var low = Low(threshold);
                _clusters.TryGetValue(high, out VanEmdeBoasTree32? cluster);
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

        /// <summary>
        /// Return last element in <see cref="VanEmdeBoasTree64"/> smaller than <paramref name="threshold"/>.
        /// </summary>
        /// <param name="threshold">Threshold value.</param>
        /// <returns>Tuple where first part is next element exist, and second part is founded element or <see cref="ulong.MinValue"/>.</returns>
        private (bool, ulong) GetPrev(ulong threshold)
        {
            if (Empty || Min >= threshold)
            {
                return (false, 0);
            }

            if (Max < threshold)
            {
                return (true, Max);
            }

            if (_summary == null || _summary.Empty)
            {
                return (true, Min);
            }
            else
            {
                var high = High(threshold);
                var low = Low(threshold);
                _clusters.TryGetValue(high, out VanEmdeBoasTree32? cluster);
                if (cluster != null && !cluster!.Empty && cluster!.Min < low)
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

        private uint High(ulong x) => (uint)(x >> 32);

        private uint Low(ulong x) => (uint)(x & uint.MaxValue);

        private ulong Merge(uint high, uint low) => ((ulong)high << 32) + low;
    }
}
