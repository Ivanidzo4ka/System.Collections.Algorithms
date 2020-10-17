namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Data structure which allow answer question on value of certain operation on interval [0..R] in O(log(n)) time.
    /// </summary>
    /// <typeparam name="TElement">Type of elements.</typeparam>
    /// <typeparam name="TValue">Type on which operation operate.</typeparam>
    public class FenwickTree<TElement, TValue> : IReadOnlyCollection<TElement>
    {
        private readonly TElement[] _data;
        private readonly TValue[] _tree;

        /// <summary>
        /// Initializes a new instance of the <see cref="FenwickTree{TElement, TValue}"/> class.
        /// </summary>
        /// <param name="data">Data to store in <see cref="FenwickTree{TElement, TValue}"/>.</param>
        /// <param name="operation">Operation to perform on data in <see cref="FenwickTree{TElement, TValue}"/>.</param>
        /// <param name="reverseOperation">Reverse operation on <paramref name="operation"/>.</param>
        /// <param name="selector">Function to pick <typeparamref name="TValue"/> from <typeparamref name="TValue"/>.</param>
        /// <param name="defaultValue">Default value for operation.</param>
        public FenwickTree(
            IEnumerable<TElement> data,
            Func<TValue, TValue, TValue> operation,
            Func<TValue, TValue, TValue> reverseOperation,
            Func<TElement, TValue> selector,
            TValue defaultValue = default(TValue))
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            _data = data.ToArray();
            if (_data.Length == 0)
                throw new ArgumentException("Collection is empty", nameof(data));
            _tree = new TValue[_data.Length];
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            ReverseOperation = reverseOperation ?? throw new ArgumentNullException(nameof(reverseOperation));
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            for (int i = 0; i < _data.Length; i++)
                _tree[i] = defaultValue;
            for (int i = 0; i < _data.Length; i++)
                Update(i, Selector(_data[i]));
        }

        /// <summary>
        /// Gets the number of elements in the <see cref="FenwickTree{TElement, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _data.Length;

        /// <summary>
        /// Gets operation to apply to elements of <see cref="FenwickTree{TElement, TValue}"/>.
        /// </summary>
        /// <remarks>Must be reversable.</remarks>
        public Func<TValue, TValue, TValue> Operation { get; }

        /// <summary>
        /// Gets reverse operation to match <see cref="FenwickTree{TElement, TValue}.Operation"/>.
        /// </summary>
        public Func<TValue, TValue, TValue> ReverseOperation { get; }

        /// <summary>
        /// Gets function which converts <typeparamref name="TElement"/> into <typeparamref name="TValue"/>.
        /// </summary>
        public Func<TElement, TValue> Selector { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <remarks>Set operation is O(log(N).</remarks>
        public TElement this[int index]
        {
            get
            {
                if (index >= _data.Length || index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _data[index];
            }

            set
            {
                if (index >= _data.Length || index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var inc = ReverseOperation(Selector(value), Selector(_data[index]));
                _data[index] = value;
                Update(index, inc);
            }
        }

        /// <summary>
        /// Get result of <see cref="FenwickTree{TElement, TValue}.Operation"/> performed on interval [0, pos] in <see cref="FenwickTree{TElement, TValue}"/>.
        /// </summary>
        /// <param name="pos">Right border of interval. Inclusive.</param>
        /// <returns>Result of <see cref="FenwickTree{TElement, TValue}.Operation"/> applied to all values in positions from 0 to <paramref name="pos"/>.</returns>
        /// <remarks>This operation is O(log(N)).</remarks>
        public TValue GetOperationValueOnInterval(int pos)
        {
            TValue result = _tree[pos];
            pos = (pos & (pos + 1)) - 1;
            while (pos >= 0)
            {
                result = Operation(result, _tree[pos]);
                pos = (pos & (pos + 1)) - 1;
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FenwickTree{TElement, TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="FenwickTree{TElement, TValue}"/>.</returns>
        public IEnumerator<TElement> GetEnumerator() => _data.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FenwickTree{TElement, TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="FenwickTree{TElement, TValue}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        private void Update(int pos, TValue increment)
        {
            for (; pos < _data.Length; pos = pos | (pos + 1))
                _tree[pos] = Operation(_tree[pos], increment);
        }
    }
}
