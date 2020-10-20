namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Data structure which allow answer question on value of certain operation on interval [0..R] in O(log(n)) time.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    /// <remarks>
    /// Main difference with <see cref="FenwickTree{T}"/> is this one doesn't store original collection inside.
    /// Benefit of that is less memory consumption, but it come with cost of O(log(N)) operation to get original value.
    /// </remarks>
    public class FenwickTreeSlim<T> : IReadOnlyCollection<T>
        where T : struct
    {
        private readonly T[] _tree;
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="FenwickTreeSlim{T}"/> class.
        /// </summary>
        /// <param name="data">Data to store in <see cref="FenwickTreeSlim{T}"/>.</param>
        /// <param name="operation">Operation to perform on data in <see cref="FenwickTree{T}"/>.</param>
        /// <param name="reverseOperation">Reverse operation on <paramref name="operation"/>.</param>
        /// <param name="defaultValue">Default value for operation.</param>
        public FenwickTreeSlim(
            IEnumerable<T> data,
            Func<T, T, T> operation,
            Func<T, T, T> reverseOperation,
            T defaultValue = default(T))
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            var size = data.Count();
            if (size == 0)
                throw new ArgumentException("Collection is empty", nameof(data));
            _tree = new T[size + 1];
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            ReverseOperation = reverseOperation ?? throw new ArgumentNullException(nameof(reverseOperation));
            Count = size;
            _version = 0;
            for (int i = 0; i < size + 1; i++)
                _tree[i] = defaultValue;
            var index = 0;
            foreach (T elem in data)
            {
                Add(++index, elem);
            }
        }

        /// <summary>
        /// Gets the number of elements in the <see cref="FenwickTreeSlim{ T}"/>.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count { get; }

        /// <summary>
        /// Gets operation to apply to elements of <see cref="FenwickTreeSlim{T}"/>.
        /// </summary>
        /// <remarks>Must be reversable.</remarks>
        public Func<T, T, T> Operation { get; }

        /// <summary>
        /// Gets reverse operation to match <see cref="FenwickTreeSlim{T}.Operation"/>.
        /// </summary>
        public Func<T, T, T> ReverseOperation { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <remarks>
        /// Get operation is O(Log(N)).
        /// Set operation is O(log(N)).
        /// </remarks>
        public T this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                index++;
                return GetValue(index);
            }

            set
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                index++;
                var inc = ReverseOperation(value, GetValue(index));
                Add(index, inc);
            }
        }

        /// <summary>
        /// Set value of <paramref name="index"/> element to result of applying <see cref="FenwickTreeSlim{T}.Operation"/> to it and <paramref name="value"/>).
        /// </summary>
        /// <param name="index">Index of element.</param>
        /// <param name="value">Value to apply.</param>
        /// <remarks>This is O(log(n)) operation.</remarks>
        public void ApplyOperationToElement(int index, T value)
        {
            if (index >= Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            index++;
            Add(index, value);
        }

        /// <summary>
        /// Get result of <see cref="FenwickTreeSlim{T}.Operation"/> performed on interval [0, pos] in <see cref="FenwickTreeSlim{T}"/>.
        /// </summary>
        /// <param name="pos">Right border of interval. Inclusive.</param>
        /// <returns>Result of <see cref="FenwickTreeSlim{T}.Operation"/> applied to all values in positions from 0 to <paramref name="pos"/>.</returns>
        /// <remarks>This operation is O(log(N)).</remarks>
        public T GetOperationValueOnInterval(int pos)
        {
            pos++;
            if (pos >= _tree.Length || pos < 1)
                throw new ArgumentOutOfRangeException(nameof(pos));
            T result = _tree[pos];
            pos -= pos & -pos;
            while (pos > 0)
            {
                result = Operation(result, _tree[pos]);
                pos -= pos & -pos;
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FenwickTreeSlim{T}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="FenwickTreeSlim{T}"/>.</returns>
        /// <remarks>This takes O(N*log(N)) time.</remarks>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Add(int pos, T increment)
        {
            _version++;
            while (pos <= Count)
            {
                _tree[pos] = Operation(_tree[pos], increment);
                pos += pos & -pos;
            }
        }

        private T GetValue(int pos)
        {
            T result = _tree[pos];
            if (pos > 0)
            {
                int z = pos - (pos & -pos);
                pos--;
                while (pos != z)
                {
                    result = ReverseOperation(result, _tree[pos]);
                    pos -= pos & -pos;
                }
            }

            return result;
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="FenwickTreeSlim{T}"/> object.
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly FenwickTreeSlim<T> _tree;
            private readonly int _version;
            private T _current;
            private int pos;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="tree">Tree to iterate.</param>
            internal Enumerator(FenwickTreeSlim<T> tree)
            {
                _tree = tree;
                _version = tree._version;
                _current = default;
                pos = 0;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public T Current => _current;

            /// <inheritdoc/>
            object? IEnumerator.Current
            {
                get
                {
                    if (pos == 0)
                        throw new Exception($"{nameof(MoveNext)} should be called first");
                    return Current;
                }
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                // Make sure that the underlying subset has not been changed since
                if (_version != _tree._version)
                {
                    throw new InvalidOperationException($"{nameof(FenwickTreeSlim<T>)} changed during enumeration.");
                }

                pos++;
                if (pos > _tree.Count)
                    return false;

                _current = _tree.GetValue(pos);
                return true;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
            }

            /// <inheritdoc/>
            void IEnumerator.Reset() => Reset();

            private void Reset()
            {
                if (_version != _tree._version)
                {
                    throw new InvalidOperationException($"{nameof(FenwickTreeSlim<T>)} changed during enumeration.");
                }

                pos = 0;
                _current = default;
            }
        }
    }
}
