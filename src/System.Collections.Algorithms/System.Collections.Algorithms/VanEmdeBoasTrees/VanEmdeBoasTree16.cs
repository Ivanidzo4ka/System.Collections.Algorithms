namespace System.Collections.Algorithms
{
    /// <summary>
    /// Van Emde Boas tree for dimensionality of <see cref="ushort"/>.
    /// </summary>
    public class VanEmdeBoasTree16
    {
        private VanEmdeBoasTree8?[] _clusters;
        private VanEmdeBoasTree8? _summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="VanEmdeBoasTree16"/> class.
        /// </summary>
        public VanEmdeBoasTree16()
        {
            Min = ushort.MaxValue;
            Count = 0;
            _clusters = new VanEmdeBoasTree8[1 << 8];
        }

        /// <summary>
        /// Gets minimum element in a tree.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// In case if tree empty returns <see cref="ushort.MaxValue"/>.
        /// </remarks>
        public ushort Min { get; private set; }

        /// <summary>
        /// Gets maximum element in a tree.
        /// </summary>
        /// <remarks>
        /// This is O(1) operation.
        /// In case if tree empty returns <see cref="ushort.MinValue"/>.
        /// </remarks>
        public ushort Max { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this tree empty or not.
        /// </summary>
        public bool Empty => Count == 0;

        /// <summary>
        /// Gets the number of items that are contained in a <see cref="VanEmdeBoasTree16"/>.
        /// </summary>
        public uint Count { get; private set; }

        /// <summary>
        /// Adds item to <see cref="VanEmdeBoasTree16"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 16) operation.
        /// </remarks>
        /// <param name="item">Item to add to <see cref="VanEmdeBoasTree16"/>.</param>
        /// <returns><see langword="true"/> if item been added, and <see langword="false"/> if <see cref="VanEmdeBoasTree16"/> already had such item.</returns>
        public bool Add(ushort item)
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
                var cluster = _clusters[high] ?? new VanEmdeBoasTree8();
                if (cluster.Empty)
                {
                    _summary = _summary ?? new VanEmdeBoasTree8();
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
        /// Searches for item in <see cref="VanEmdeBoasTree16"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 16) operation.
        /// </remarks>
        /// <param name="item">Item to search in <see cref="VanEmdeBoasTree16"/>.</param>
        /// <returns><see langword="true"/> if item is present, and <see langword="false"/> if not present.</returns>
        public bool Find(ushort item)
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

        /// <summary>
        /// Trys to get next value bigger than <paramref name="value"/> in <see cref="VanEmdeBoasTree16"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 16) operation.
        /// <paramref name="value"/> Doesn't have to be present in <see cref="VanEmdeBoasTree16"/>.
        /// </remarks>
        /// <param name="value">Looking for item bigger than this one.</param>
        /// <param name="result">Item bigger than <paramref name="value"/>> if it exist, <see cref="ushort.MaxValue"/> otherwise.</param>
        /// <returns><see langword="true"/> if such item exist, and <see langword="false"/> if not.</returns>
        public bool TryGetNext(ushort value, out ushort result)
        {
            var (found, ans) = GetNext(value);
            result = ans;
            return found;
        }

        /// <summary>
        /// Trys to get next value smaller than <paramref name="value"/> in <see cref="VanEmdeBoasTree16"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 16) operation.
        /// <paramref name="value"/> Doesn't have to be present in <see cref="VanEmdeBoasTree16"/>.
        /// </remarks>
        /// <param name="value">Looking for item smaller than this one.</param>
        /// <param name="result">Item smaller than <paramref name="value"/>> if it exist, <see cref="ushort.MinValue"/> otherwise.</param>
        /// <returns><see langword="true"/> if such item exist, and <see langword="false"/> if not.</returns>
        public bool TryGetPrevious(ushort value, out ushort result)
        {
            var (found, ans) = GetPrev(value);
            result = ans;
            return found;
        }

        /// <summary>
        /// Remove item from <see cref="VanEmdeBoasTree16"/>.
        /// </summary>
        /// <remarks>
        /// This is O(log 16) operation.
        /// </remarks>
        /// <param name="item">Item to remove from <see cref="VanEmdeBoasTree16"/>.</param>
        /// <returns><see langword="true"/> if item been removed, and <see langword="false"/> if <see cref="VanEmdeBoasTree16"/> didn't had it.</returns>
        public bool Remove(ushort item)
        {
            if (Empty)
                return false;

            if (Min == item && Max == item)
            {
                Min = ushort.MaxValue;
                Max = 0;
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
            var cluster = _clusters[high];
            if (cluster == null)
                return false;
            var removed = cluster!.Remove(low);
            if (cluster!.Empty)
            {
                _clusters[high] = default;
                _summary.Remove(high);
            }

            if (removed)
                Count--;
            return removed;
        }

        /// <summary>
        /// Return first element in <see cref="VanEmdeBoasTree16"/> bigger than <paramref name="threshold"/>.
        /// </summary>
        /// <param name="threshold">Threshold value.</param>
        /// <returns>Tuple where first part is next element exist, and second part is founded element or <see cref="ushort.MaxValue"/>.</returns>
        internal (bool, ushort) GetNext(ushort threshold)
        {
            if (Empty || Max <= threshold)
            {
                return (false, ushort.MaxValue);
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

        /// <summary>
        /// Return last element in <see cref="VanEmdeBoasTree8"/> smaller than <paramref name="threshold"/>.
        /// </summary>
        /// <param name="threshold">Threshold value.</param>
        /// <returns>Tuple where first part is next element exist, and second part is founded element or <see cref="ushort.MinValue"/>.</returns>
        internal (bool, ushort) GetPrev(ushort threshold)
        {
            if (Empty || Min >= threshold)
            {
                return (false, ushort.MinValue);
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
                var cluster = _clusters[high];
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

        private byte High(ushort x) => (byte)(x >> 8);

        private byte Low(ushort x) => (byte)(x & byte.MaxValue);

        private ushort Merge(byte high, byte low) => (ushort)((high << 8) + low);
    }
}
