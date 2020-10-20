namespace System.Collections.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Data structure which allow answer question on value of certain operation on interval [L..R] in O(log(n)) time.
    /// </summary>
    /// <typeparam name="TElement">Type of elements.</typeparam>
    /// <typeparam name="TValue">Type on which operation operate.</typeparam>
    /// <remarks>Also known as Counter tree Fenwick. Main difference with <see cref="FenwickTree{TElement, TValue}"/> is, this one is store two trees and able to answer questions on interval [L..R] rather than [0..R].</remarks>
    public class BinaryIndexedTree<TElement, TValue> : IReadOnlyCollection<TElement>
    {
        private TElement[] _data;
        private TValue[] _left;
        private TValue[] _right;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryIndexedTree{TElement, TValue}"/> class.
        /// </summary>
        /// <param name="data">Data to store in <see cref="BinaryIndexedTree{TElement, TValue}"/>.</param>
        /// <param name="operation">Operation to perform on data in <see cref="BinaryIndexedTree{TElement, TValue}"/>.</param>
        /// <param name="reverseOperation">Reverse operation on <paramref name="operation"/>.</param>
        /// <param name="selector">Function to pick <typeparamref name="TValue"/> from <typeparamref name="TValue"/>.</param>
        /// <param name="defaultValue">Default value for operation.</param>
        public BinaryIndexedTree(
            IEnumerable<TElement> data,
            Func<TValue, TValue, TValue> operation,
            Func<TValue, TValue, TValue> reverseOperation,
            Func<TElement, TValue> selector,
            TValue defaultValue = default(TValue))
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
#pragma warning disable CS8604 // Possible null reference argument.
            _data = data.Prepend(default).ToArray();
#pragma warning restore CS8604 // Possible null reference argument.
            if (_data.Length == 1)
                throw new ArgumentException("Collection is empty", nameof(data));

            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            ReverseOperation = reverseOperation;
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _left = new TValue[_data.Length];
            _right = new TValue[_data.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                _left[i] = defaultValue;
                _right[i] = defaultValue;
            }

            for (int i = 1; i < _data.Length; i++)
                Update(i, Selector(_data[i]));
        }

        /// <summary>
        /// Gets the number of elements in the <see cref="BinaryIndexedTree{TElement, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _data.Length - 1;

        /// <summary>
        /// Gets operation to apply to elements of <see cref="BinaryIndexedTree{TElement, TValue}"/>.
        /// </summary>
        /// <remarks>Must be reversable.</remarks>
        public Func<TValue, TValue, TValue> Operation { get; }

        /// <summary>
        /// Gets reverse operation to match <see cref="BinaryIndexedTree{TElement, TValue}.Operation"/>.
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
                index++;
                if (index >= _data.Length || index < 1)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _data[index];
            }

            set
            {
                index++;
                if (index >= _data.Length || index < 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (ReverseOperation == null)
                    throw new NotSupportedException($"{nameof(ReverseOperation)} should be define to perform update in {nameof(BinaryIndexedTree<TElement, TValue>)} ");

                var inc = ReverseOperation(Selector(value), Selector(_data[index]));
                _data[index] = value;
                Update(index, inc);
            }
        }

        /// <summary>
        /// Get result of <see cref="BinaryIndexedTree{TElement, TValue}.Operation"/> performed on interval [0, pos] in <see cref="BinaryIndexedTree{TElement, TValue}"/>.
        /// </summary>
        /// <param name="left">Left border of interval. Inclusive.</param>
        /// <param name="right">Right border of interval. Inclusive.</param>
        /// <returns>Result of <see cref="BinaryIndexedTree{TElement, TValue}.Operation"/> applied to all values in positions from <paramref name="left"/> to <paramref name="right"/>.</returns>
        /// <remarks>This operation is O(log(N)).</remarks>
        public TValue GetOperationValueOnInterval(int left, int right)
        {
            left++;
            right++;
            if (left >= _data.Length || left < 1)
                throw new ArgumentOutOfRangeException(nameof(left));
            if (right >= _data.Length || right < 1)
                throw new ArgumentOutOfRangeException(nameof(right));
            if (left > right)
                throw new ArgumentOutOfRangeException(nameof(left), $"Should be smaller or equal to {nameof(right)}");
            if (left == right)
                return Selector(_data[left]);
            if (right - left == 1)
                return Operation(Selector(_data[left]), Selector(_data[right]));

            var (leftTreeClimb, common) = ClimbTree(_right, left, (x) => x + Utils.IsolateLastBit(x), (x) => x <= right);
            var (rightTreeClimb, _) = ClimbTree(_left, right, (x) => x - Utils.IsolateLastBit(x), (x) => x >= left);
            var result = Operation(leftTreeClimb, rightTreeClimb);
            return Operation(result, Selector(_data[common]));
        }

        /// <summary>
        /// Set value of <paramref name="index"/> element to result of applying <see cref="BinaryIndexedTree{TElement, TValue}.Operation"/> to it and <paramref name="value"/>).
        /// </summary>
        /// <param name="index">Index of element.</param>
        /// <param name="value">Value to apply.</param>
        /// <remarks>This is O(log(n)) operation.</remarks>
        public void ApplyOperationToElement(int index, TElement value)
        {
            if (index >= Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            index++;
            Update(index, Selector(value));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BinaryIndexedTree{TElement, TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="BinaryIndexedTree{TElement, TValue}"/>.</returns>
        public IEnumerator<TElement> GetEnumerator() => _data.AsEnumerable().Skip(1).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BinaryIndexedTree{TElement, TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the contents of the  <see cref="BinaryIndexedTree{TElement, TValue}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _data.Skip(1).GetEnumerator();

        private (TValue, int) ClimbTree(TValue[] tree, int pos, Func<int, int> treeClimb, Func<int, bool> climbStop)
        {
            int up = pos;
            int nextup = treeClimb(pos);
            if (!climbStop(nextup))
                return (tree[0], up);
            TValue result = tree[up];
            while (true)
            {
                up = nextup;
                nextup = treeClimb(up);
                if (!climbStop(nextup))
                    break;
                result = Operation(result, tree[up]);
            }

            return (result, up);
        }

        private void Update(int index, TValue increment)
        {
            var orignal = index;
            while (index < _data.Length)
            {
                _left[index] = Operation(_left[index], increment);
                index += Utils.IsolateLastBit(index);
            }

            index = orignal;
            while (index > 0)
            {
                _right[index] = Operation(_right[index], increment);
                index -= Utils.IsolateLastBit(index);
            }
        }
    }
}
