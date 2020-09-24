using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Algorithms
{
    public static class Extensions
    {
        public static void Swap<T>(this IList<T> array, int left, int right)
        {
            T temp = array[left];
            array[left] = array[right];
            array[right] = temp;
        }

        public static int Partition<T>(this IList<T> array, int index, int lenght, T element)
        {
            return array.Partition(index, lenght, element, Comparer<T>.Default);
        }

        public static int Partition<T>(this IList<T> array, int index, int lenght, T element, IComparer<T> comparer)
        {
            int i;
            for (i = index; i < index + lenght - 1; i++)
            {
                if (comparer.Compare(array[i], element) == 0)
                    break;
            }

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

        public static T KthElement<T>(this IEnumerable<T> data, int k)
        {
            var array = data.ToArray();
            return KthElement(array, 0, array.Length - 1, k, Comparer<T>.Default);
        }

        public static T KthElement<T>(this IEnumerable<T> data, int k, IComparer<T> comparer)
        {
            var array = data.ToArray();
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
            int pos = Partition(array, left, n, medianOfMedians);

            if (pos - left == k)
                return array[pos];
            if (pos - left > k)
                return KthElement(array, left, pos - 1, k, comparer);
            return KthElement(array, pos + 1, right, k - pos + left - 1, comparer);
        }
    }
}
