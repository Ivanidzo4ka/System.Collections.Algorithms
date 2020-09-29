namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Linq;

    public class SqrtTree<T>
    {
        private List<T[]> _prefix;
        private List<T[]> _suffix;

        // TODO:
        // Maybe it should be collection of sparse arrays.
        // First few layers have about 65% of zeros.
        private List<T[]> _between;
        private List<int> _layers;
        private int[] _innerTreeLayers;
        private T[] _data;
        private int _originalSize;
        private int _log;
        private int _indexSize;

        private Func<T, T, T> _operation;

        public SqrtTree(int capacity, Func<T, T, T> operation)
            : this(CapacityCheck(capacity), operation)
        {
        }

        public SqrtTree(IEnumerable<T> collection, Func<T, T, T> operation)
        : this(ConvertFromIEnumerable(collection), operation)
        {
        }

        public SqrtTree(T[] collection, Func<T, T, T> operation)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            if (operation is null)
                throw new ArgumentNullException(nameof(operation));
            _data = collection;
            _originalSize = _data.Length;
            _operation = operation;
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

            Build(0, 0, _originalSize, 0);
        }

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_originalSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _data[index];
            }

            set
            {
                if ((uint)index >= (uint)_originalSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _data[index] = value;
                Update(0, 0, _originalSize, 0, index);
            }
        }

        public T Query(int left, int right)
        {
            if (left < 0 || left >= _originalSize)
                throw new ArgumentOutOfRangeException(nameof(left));
            if (right < 0 || right >= _originalSize)
                throw new ArgumentOutOfRangeException(nameof(left));
            if (left <= right)
                return Query(left, right, 0, 0);
            else
                return Query(right, left, 0, 0);
        }

        private void BuildBlock(int layer, int left, int right)
        {
            _prefix[layer][left] = _data[left];
            for (int i = left + 1; i < right; i++)
            {
                _prefix[layer][i] = _operation(_prefix[layer][i - 1], _data[i]);
            }

            _suffix[layer][right - 1] = _data[right - 1];
            for (int i = right - 2; i >= left; i--)
            {
                _suffix[layer][i] = _operation(_data[i], _suffix[layer][i + 1]);
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
                _data[_originalSize + i] = _suffix[0][i << betweenSizeLog];
            }

            Build(1, _originalSize, _data.Length, (1 << _log) - _originalSize);
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
                    ans = (i == j) ? add : _operation(ans, add);
                    _between[layer - 1][betweenOffset + left + (i << blockCountLog) + j] = ans;
                }
            }
        }

        private T Query(int left, int right, int betweenOffset, int offset)
        {
            if (left == right)
                return _data[left];
            if (left + 1 == right)
                return _operation(_data[left], _data[right]);
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
                    ? Query(_originalSize + leftBlock, _originalSize + rightBlock, (1 << _log) - _originalSize, _originalSize)
                    : _between[layer - 1][betweenOffset + leftPos + (leftBlock << blockCountLog) + rightBlock];
                result = _operation(result, add);
            }

            result = _operation(result, _prefix[layer][right]);
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
            _data[_originalSize + blockIndex] = _suffix[0][blockIndex << blockSizeLog];
            Update(1, _originalSize, _originalSize + _indexSize, (1 << _log) - _originalSize, _originalSize + blockIndex);
        }

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
    }
}