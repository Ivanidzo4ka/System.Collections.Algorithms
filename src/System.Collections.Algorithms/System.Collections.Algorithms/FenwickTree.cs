namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Data structure to which allow answer question on value of certain operation on interval [0..R] in O(log(n)) time.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    public class FenwickTree<T> : IReadOnlyCollection<T>
    {
        private readonly T[] _data;
        private readonly T[] _tree;

        /// <summary>
        /// Initializes a new instance of the <see cref="FenwickTree{T}"/> class.
        /// </summary>
        /// <param name="data">Data to store in <see cref="FenwickTree{T}"/>.</param>
        /// <param name="operation">Operation to perform on data in <see cref="FenwickTree{T}"/>.</param>
        /// <param name="reverseOperation">Reverse operation on <paramref name="reverseOperation"/>.</param>
        /// <param name="defaultValue">Default value for operation.</param>
        public FenwickTree(IEnumerable<T> data, Func<T, T, T> operation, Func<T, T, T> reverseOperation, T defaultValue = default(T))
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (reverseOperation == null)
                throw new ArgumentNullException(nameof(reverseOperation));
            _data = data.ToArray();
            if (_data.Length == 0)
                throw new ArgumentException("Collection is empty", nameof(data));
            _tree = new T[_data.Length];
            Operation = operation;
            ReverseOperation = reverseOperation;
            for (int i = 0; i < _data.Length; i++)
                _tree[i] = defaultValue;
            for (int i = 0; i < _data.Length; i++)
                Update(i, _data[i]);
        }

        /// <summary>
        /// Gets the number of elements in the <see cref="FenwickTree{T}"/>.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _data.Length;

        /// <summary>
        /// Gets operation to apply to elements of <see cref="FenwickTree{T}"/>.
        /// </summary>
        /// <remarks>Must be reversable.</remarks>
        public Func<T, T, T> Operation { get; }

        /// <summary>
        /// Gets reverse operation to match <see cref="FenwickTree{T}.Operation"/>.
        /// </summary>
        public Func<T, T, T> ReverseOperation { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <remarks>Set operation is O(log(N).</remarks>
        public T this[int index]
        {
            get
            {
                if (index >= _data.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _data[index];
            }

            set
            {
                if (index >= _data.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var inc = ReverseOperation(value, _data[index]);
                _data[index] = value;
                Update(index, inc);
            }
        }

        /// <summary>
        /// Get result of <see cref="FenwickTree{T}.Operation"/> performed on interval [0, pos] in <see cref="FenwickTree{T}"/>.
        /// </summary>
        /// <param name="pos">Right border of interval. Inclusive.</param>
        /// <returns>Result of <see cref="FenwickTree{T}.Operation"/> applied to all values in positions from 0 to <paramref name="pos"/>.</returns>
        /// <remarks>This operation is O(log(N)).</remarks>
        public T GetOperationValueOnInterval(int pos)
        {
            T result = _tree[pos];
            pos = (pos & (pos + 1)) - 1;
            while (pos >= 0)
            {
                result = Operation(result, _tree[pos]);
                pos = (pos & (pos + 1)) - 1;
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FenwickTree{T}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="FenwickTree{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator() => _data.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FenwickTree{T}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="FenwickTree{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        private void Update(int pos, T increment)
        {
            for (; pos < _data.Length; pos = pos | (pos + 1))
                _tree[pos] = Operation(_tree[pos], increment);
        }
    }
}
