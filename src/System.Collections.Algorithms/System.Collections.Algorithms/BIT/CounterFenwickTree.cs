using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Algorithms
{
    public class CounterFenwickTree<T>
        where T : struct
    {
        private T[] _tree;
        private T[] _oppositeTree;
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterFenwickTree{T}"/> class.
        /// </summary>
        /// <param name="data">Data to store in <see cref="CounterFenwickTree{T}"/>.</param>
        /// <param name="operation">Operation to perform on data in <see cref="CounterFenwickTree{T}"/>.</param>
        /// <param name="reverseOperation">Reverse operation on <paramref name="operation"/>.</param>
        /// <param name="defaultValue">Default value for operation.</param>
        public CounterFenwickTree(
            IEnumerable<T> data,
            Func<T, T, T> operation,
            Func<T, T, T> reverseOperation,
            T defaultValue = default(T))
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            _tree = data.Prepend(defaultValue).ToArray();
            if (_tree.Length == 1)
                throw new ArgumentException("Collection is empty", nameof(data));
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            ReverseOperation = reverseOperation ?? throw new ArgumentNullException(nameof(reverseOperation));
            Count = _tree.Length - 1;
            DefaultValue = defaultValue;
            _oppositeTree = new T[_tree.Length];
            _version = 0;
            for (int i = 1; i < _tree.Length - 1; i++)
            {
                _oppositeTree[i] = _tree[i + 1];
            }

            for (int i = 1; i < _tree.Length; i++)
            {
                int pos = i + (i & -i);
                if (pos < _tree.Length)
                    _tree[pos] = Operation(_tree[i], _tree[pos]);
            }

            for (int i = 1; i < _tree.Length - 1; i++)
            {
                int reverseI = _tree.Length - i - 1;
                int pos = i + (i & -i);
                int reversePos = _tree.Length - pos - 1;
                if (reversePos > 1)
                    _oppositeTree[reversePos] = Operation(_oppositeTree[reverseI], _oppositeTree[reversePos]);
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

        public T DefaultValue { get; }

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


        public T Ask(int l, int r)
        {
            T res = default;
            for (; (r & r + 1) >= l && r >= l; r = (r & r + 1) - 1)
                res = Operation(_tree[r + 1], res);
            for (; (l | l - 1) <= r && l > 0 && l < Count; l = (l | l - 1) + 1)
                res = Operation(_oppositeTree[l], res);
            return res;
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

    }
}
