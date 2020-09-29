namespace System.Collections.Algorithms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Sqrt tree applies sqrt decomposition to provided collection and allow to query collection for specified range by provided associative operation.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    public class SqrtTree<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly List<T[]> _prefix;
        private readonly List<T[]> _suffix;

        // TODO:
        // Maybe it should be collection of sparse arrays.
        // First few layers have about 65% of zeros.
        private readonly List<T[]> _between;

        private readonly List<int> _layers;
        private readonly int[] _innerTreeLayers;
        private readonly T[] _data;
        private readonly int _log;
        private readonly int _indexSize;

        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqrtTree{T}"/> class with specified size.
        /// </summary>
        /// <param name="size">Size of collection.</param>
        /// <param name="operation">Associative operation on <typeparamref name="T"/>.</param>
        public SqrtTree(int size, Func<T, T, T> operation)
            : this(CapacityCheck(size), operation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqrtTree{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the <see cref="SqrtTree{T}"/>.</param>
        /// <param name="operation">Associative operation on <typeparamref name="T"/>.</param>
        public SqrtTree(IEnumerable<T> collection, Func<T, T, T> operation)
        : this(ConvertFromIEnumerable(collection), operation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqrtTree{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the <see cref="SqrtTree{T}"/>.</param>
        /// <param name="operation">Associative operation on <typeparamref name="T"/>.</param>
        public SqrtTree(T[] collection, Func<T, T, T> operation)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            if (operation is null)
                throw new ArgumentNullException(nameof(operation));
            _data = collection;
            Count = _data.Length;
            Operation = operation;
            _log = Utils.Log2(_data.Length);
            _innerTreeLayers = new int[_log + 1];

            var tmp = _log;
            _layers = new List<int>();
            while (tmp > 1)
            {
                _innerTreeLayers[tmp] = _layers.Count;
                _layers.Add(tmp);
                tmp = (tmp + 1) >> 1;
            }

            for (int i = _log - 1; i >= 0; i--)
            {
                _innerTreeLayers[i] = Math.Max(_innerTreeLayers[i], _innerTreeLayers[i + 1]);
            }

            int betweenLayers = Math.Max(0, _layers.Count - 1);
            int blockSizeLog = (_log + 1) >> 1;
            int blockSize = 1 << blockSizeLog;
            _indexSize = (_data.Length + blockSize - 1) >> blockSizeLog;

            Array.Resize(ref _data, _data.Length + _indexSize);

            _prefix = new List<T[]>(_layers.Count);
            for (int i = 0; i < _layers.Count; i++)
                _prefix.Add(new T[_data.Length]);

            _suffix = new List<T[]>(_layers.Count);
            for (int i = 0; i < _layers.Count; i++)
                _suffix.Add(new T[_data.Length]);

            _between = new List<T[]>(betweenLayers);
            for (int i = 0; i < betweenLayers; i++)
                _between.Add(new T[(1 << _log) + blockSize]);

            Build(0, 0, Count, 0);
        }

        /// <summary>
        /// Gets the number of elements in the <see cref="SqrtTree{T}"/>.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets associative operation on <see cref="SqrtTree{T}"/>.
        /// </summary>
        public Func<T, T, T> Operation { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <remarks>Set operation is O(sqrt(N).</remarks>
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _data[index];
            }

            set
            {
                if ((uint)index >= (uint)Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _version++;
                _data[index] = value;
                Update(0, 0, Count, 0, index);
            }
        }

        /// <summary>
        /// Gets result of associative operation on collection in range [<paramref name="left"/>"/>, <paramref name="right"/>].
        /// </summary>
        /// <param name="left">Left index of range in collection.</param>
        /// <param name="right">Right index of range in collection.</param>
        /// <returns>
        /// Result of associative operation on collection within specified range.
        /// </returns>
        /// <remarks>
        /// This operation is O(1).
        /// </remarks>>
        public T Query(int left, int right)
        {
            if (left < 0 || left >= Count)
                throw new ArgumentOutOfRangeException(nameof(left));
            if (right < 0 || right >= Count)
                throw new ArgumentOutOfRangeException(nameof(left));
            if (left <= right)
                return Query(left, right, 0, 0);
            else
                return Query(right, left, 0, 0);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static T[] CapacityCheck(int capacity)
        {
            if (capacity <= 0 || capacity > 1 << 30)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            return new T[capacity];
        }

        private static T[] ConvertFromIEnumerable(IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            var data = collection.ToArray();
            if (data.Length == 0)
                throw new ArgumentException(nameof(collection), "Is empty");
            return data;
        }

        private void BuildBlock(int layer, int left, int right)
        {
            _prefix[layer][left] = _data[left];
            for (int i = left + 1; i < right; i++)
            {
                _prefix[layer][i] = Operation(_prefix[layer][i - 1], _data[i]);
            }

            _suffix[layer][right - 1] = _data[right - 1];
            for (int i = right - 2; i >= left; i--)
            {
                _suffix[layer][i] = Operation(_data[i], _suffix[layer][i + 1]);
            }
        }

        private void Build(int layer, int left, int right, int betweenOffest)
        {
            if (layer >= _layers.Count)
                return;
            int betweenSize = 1 << ((_layers[layer] + 1) >> 1);
            for (int leftPos = left; leftPos < right; leftPos += betweenSize)
            {
                int rightPos = Math.Min(leftPos + betweenSize, right);
                BuildBlock(layer, leftPos, rightPos);
                Build(layer + 1, leftPos, rightPos, betweenOffest);
            }

            if (layer == 0)
                BuildBetweenZero();
            else
                BuildBetween(layer, left, right, betweenOffest);
        }

        private void BuildBetweenZero()
        {
            int betweenSizeLog = (_log + 1) >> 1;
            for (int i = 0; i < _indexSize; i++)
            {
                _data[Count + i] = _suffix[0][i << betweenSizeLog];
            }

            Build(1, Count, _data.Length, (1 << _log) - Count);
        }

        private void BuildBetween(int layer, int left, int right, int betweenOffset)
        {
            int blocSizeLog = (_layers[layer] + 1) >> 1;
            int blockCountLog = _layers[layer] >> 1;
            int blockSize = 1 << blocSizeLog;
            int blockCount = (right - left + blockSize - 1) >> blocSizeLog;
            for (int i = 0; i < blockCount; i++)
            {
                T ans = default;
                for (int j = i; j < blockCount; j++)
                {
                    T add = _suffix[layer][left + (j << blocSizeLog)];
                    ans = (i == j) ? add : Operation(ans, add);
                    _between[layer - 1][betweenOffset + left + (i << blockCountLog) + j] = ans;
                }
            }
        }

        private T Query(int left, int right, int betweenOffset, int offset)
        {
            if (left == right)
                return _data[left];
            if (left + 1 == right)
                return Operation(_data[left], _data[right]);
            var diff = (left - offset) ^ (right - offset);

            int layer = _innerTreeLayers[diff == 0 ? 0 : Utils.Log2(diff + 1)];
            int blockSizeLog = (_layers[layer] + 1) >> 1;
            int blockCountLog = _layers[layer] >> 1;
            int leftPos = (((left - offset) >> _layers[layer]) << _layers[layer]) + offset;
            int leftBlock = ((left - leftPos) >> blockSizeLog) + 1;
            int rightBlock = ((right - leftPos) >> blockSizeLog) - 1;
            T result = _suffix[layer][left];
            if (leftBlock <= rightBlock)
            {
                T add = layer == 0
                    ? Query(Count + leftBlock, Count + rightBlock, (1 << _log) - Count, Count)
                    : _between[layer - 1][betweenOffset + leftPos + (leftBlock << blockCountLog) + rightBlock];
                result = Operation(result, add);
            }

            result = Operation(result, _prefix[layer][right]);
            return result;
        }

        private void Update(int layer, int left, int right, int betweenOffset, int indexOffset)
        {
            if (layer >= _layers.Count)
                return;
            int blockSizeLog = (_layers[layer] + 1) >> 1;
            int blockSize = 1 << blockSizeLog;
            int blockIndex = (indexOffset - left) >> blockSizeLog;
            int leftPos = left + (blockIndex << blockSizeLog);
            int rightPos = Math.Min(leftPos + blockSize, right);
            BuildBlock(layer, leftPos, rightPos);
            if (layer == 0)
                UpdateBetweenZero(blockIndex);
            else
                BuildBetween(layer, left, right, betweenOffset);
            Update(layer + 1, leftPos, rightPos, betweenOffset, indexOffset);
        }

        private void UpdateBetweenZero(int blockIndex)
        {
            int blockSizeLog = (_log + 1) >> 1;
            _data[Count + blockIndex] = _suffix[0][blockIndex << blockSizeLog];
            Update(1, Count, Count + _indexSize, (1 << _log) - Count, Count + blockIndex);
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="SqrtTree{T}{T}"/> object.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            /// <summary>The span being enumerated.</summary>
            private readonly SqrtTree<T> _tree;
            private readonly int _version;

            /// <summary>The next index to yield.</summary>
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
            /// <param name="tree">The Sqrt-tree to enumerate.</param>
            internal Enumerator(SqrtTree<T> tree)
            {
                _tree = tree ?? throw new ArgumentNullException(nameof(tree));
                _version = tree._version;
                _index = -1;
            }

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public T Current => _tree._data[_index];

            /// <inheritdoc/>
            object? IEnumerator.Current => _tree._data[_index];

            /// <inheritdoc/>
            public bool MoveNext()
            {
                ValidateVersion();
                int index = _index + 1;
                if (index < _tree.Count)
                {
                    _index = index;
                    return true;
                }

                return false;
            }

            /// <inheritdoc/>
            public void Reset()
            {
                ValidateVersion();
                _index = 0;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
            }

            private void ValidateVersion()
            {
                if (_version != _tree._version)
                {
                    throw new InvalidOperationException("SqrtTree changed during enumeration.");
                }
            }
        }
    }
}