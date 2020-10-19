namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Data structure which allow answer question on value of certain operation on interval [0..R] in O(log(n)) time.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    public class FenwickTree<T> : FenwickTree<T, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FenwickTree{T}"/> class.
        /// </summary>
        /// <param name="data">Data to store in <see cref="FenwickTree{T}"/>.</param>
        /// <param name="operation">Operation to perform on data in <see cref="FenwickTree{T}"/>.</param>
        /// <param name="reverseOperation">Reverse operation on <paramref name="operation"/>.</param>
        /// <param name="defaultValue">Default value for operation.</param>
        public FenwickTree(
            IEnumerable<T> data,
            Func<T, T, T> operation,
            Func<T, T, T> reverseOperation,
            T defaultValue = default(T))
            : base(data, operation, reverseOperation, Identity, defaultValue)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Identity(T x) => x;
    }
}
