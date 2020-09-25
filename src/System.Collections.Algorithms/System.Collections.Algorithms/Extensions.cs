namespace System.Collections.Algorithms
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class providing extended functionality for already existing structures.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Swap objects in array.
        /// </summary>
        /// <typeparam name="T">Type of objects in array.</typeparam>
        /// <param name="array">Array to operate on.</param>
        /// <param name="first">Index of first element to be swaped.</param>
        /// <param name="second">Index of second element to be swaped.</param>
        public static void Swap<T>(this IList<T> array, int first, int second)
        {
            T temp = array[first];
            array[first] = array[second];
            array[second] = temp;
        }

        /// <summary>
        /// Partition array into two parts, first part contains values less than element, second half start with element and contains rest of array.
        /// </summary>
        /// <typeparam name="T">Type of objects in array.</typeparam>
        /// <param name="array">Array to operate on.</param>
        /// <param name="index">The starting index of range to partition.</param>
        /// <param name="lenght">The number of objects in array to partition.</param>
        /// <param name="element">Element in array to parition on.</param>
        /// <returns>Position of element in partitioned array.</returns>
        public static int Partition<T>(this IList<T> array, int index, int lenght, T element)
        {
            return array.Partition(index, lenght, element, Comparer<T>.Default);
        }

        /// <summary>
        /// Partition array into two parts, first part contains values less for which specified <see cref="IComparer{T}"/> returns -1, second half start with element and contains rest of array.
        /// </summary>
        /// <typeparam name="T">Type of objects in array.</typeparam>
        /// <param name="array">Array to operate on.</param>
        /// <param name="index">The starting index of range to partition.</param>
        /// <param name="lenght">The number of objects in array to partition.</param>
        /// <param name="element">Element in array to parition on.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements.</param>
        /// <returns>Position of element in partitioned array.</returns>
        public static int Partition<T>(this IList<T> array, int index, int lenght, T element, IComparer<T> comparer)
        {
            int i;
            for (i = index; i < index + lenght - 1; i++)
            {
                if (comparer.Compare(array[i], element) == 0)
                    break;
            }

            if (comparer.Compare(array[i], element) != 0)
                throw new ArgumentOutOfRangeException(nameof(element));

            array.Swap(i, index + lenght - 1);
            i = index;
            for (int j = index; j <= index + lenght - 2; j++)
            {
                if (comparer.Compare(array[j], element) == -1)
                {
                    array.Swap(i, j);
                    i++;
                }
            }

            array.Swap(i, index + lenght - 1);
            return i;
        }

        /// <summary>
        /// Returns k-th object in enumeration if enumeration been sorted.
        /// </summary>
        /// <typeparam name="T">Type of objects in array.</typeparam>
        /// <param name="data">Data to find k-th object.</param>
        /// <param name="k">K. Zero based, shouldn't exceed size of enumeration.</param>
        /// <returns>K-th object in enumeration.</returns>
        public static T KthElement<T>(this IEnumerable<T> data, int k)
        {
            var array = data.ToArray();
            return KthElement(array, 0, array.Length - 1, k, Comparer<T>.Default);
        }

        /// <summary>
        /// Finds k-th object in enumeration if enumeration been sorted according to <see cref="IComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of objects in array.</typeparam>
        /// <param name="data">Data to find k-th object.</param>
        /// <param name="k">K. Zero based, shouldn't exceed size of enumeration.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements.</param>
        /// <returns>K-th object in enumeration.</returns>
        /// <remarks>Finding k-th object is O(n) operation.</remarks>
        public static T KthElement<T>(this IEnumerable<T> data, int k, IComparer<T> comparer)
        {
            var array = data.ToArray();
            if (k < 0 || k >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(k));
            return KthElement(array, 0, array.Length - 1, k, comparer);
        }

        private static T FindMedian<T>(IList<T> data, int index, int lenght, IComparer<T> comparer)
        {
            ArrayList.Adapter((IList)data).Sort(index, lenght, (IComparer)comparer);
            return data[index + (lenght / 2)];
        }

        private static T KthElement<T>(T[] array, int left, int right, int k, IComparer<T> comparer)
        {
            int n = right - left + 1;
            var medians = new T[(n + 4) / 5];
            int i;
            for (i = 0; i < n / 5; i++)
                medians[i] = FindMedian(array, left + (i * 5), 5, comparer);
            if (i * 5 < n)
            {
                medians[i] = FindMedian(array, left + (i * 5), n % 5, comparer);
                i++;
            }

            T medianOfMedians = (i == 1) ? medians[i - 1] :
                                KthElement(medians, 0, i - 1, i / 2, comparer);
            int pos = Partition(array, left, n, medianOfMedians, comparer);

            if (pos - left == k)
                return array[pos];
            if (pos - left > k)
                return KthElement(array, left, pos - 1, k, comparer);
            return KthElement(array, pos + 1, right, k - pos + left - 1, comparer);
        }
    }
}
