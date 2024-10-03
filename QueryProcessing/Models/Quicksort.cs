using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessing.Models
{
    internal class Quicksort
    {
        public static List<object[]> sortManager (List<object[]> matrix, int colIndex, bool ascending)
        {
            if (Equals(matrix[0][colIndex].GetType(), typeof(int)))
                sort<int>(matrix, 0, matrix.Count() - 1, colIndex, ascending);
            else if (Equals(matrix[0][colIndex].GetType(), typeof(string)))
                sort<string>(matrix, 0, matrix.Count() - 1, colIndex, ascending);
            else if (Equals(matrix[0][colIndex].GetType(), typeof(DateTime)))
                sort<DateTime>(matrix, 0, matrix.Count() - 1, colIndex, ascending);
            else sort<double>(matrix, 0, matrix.Count() - 1, colIndex, ascending);

            return matrix;
        } 

        public static void sort<T>(List<object[]> matrix, int left, int right, int colIndex, bool ascending) where T : IComparable<T>
        {
            if (left < right)
            {
                int pivot = partition<T>(matrix, left, right, colIndex, ascending);
                sort<T>(matrix, left, pivot - 1, colIndex, ascending);
                sort<T>(matrix, pivot + 1, right, colIndex, ascending);
            }
        }

        public static int partition<T>(List<object[]> matrix, int left, int right, int colIndex, bool ascending) where T : IComparable<T>
        {

            T pivot = (T)matrix[right][colIndex];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                T currentValue = (T)matrix[j][colIndex];

                if (ascending ? currentValue.CompareTo(pivot) <= 0 : currentValue.CompareTo(pivot) >= 0)
                {
                    i++;
                    var temp = matrix[i];
                    matrix[i] = matrix[j];
                    matrix[j] = temp;
                }
            }

            var tempPivot = matrix[i + 1];
            matrix[i + 1] = matrix[right];
            matrix[right] = tempPivot;

            return i + 1;
        }


    }
}
