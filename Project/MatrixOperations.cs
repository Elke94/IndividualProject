using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    class MatrixOperations
    {

        public double[,] TransposeRowsAndColumns(double[,] arr)
        {
            int rowCount = arr.GetLength(0);
            int columnCount = arr.GetLength(1);
            double[,] transposed = new double[columnCount, rowCount];
            if (rowCount == columnCount)
            {
                transposed = (double[,])arr.Clone();
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        double temp = transposed[i, j];
                        transposed[i, j] = transposed[j, i];
                        transposed[j, i] = temp;
                    }
                }
            }
            else
            {
                for (int column = 0; column < columnCount; column++)
                {
                    for (int row = 0; row < rowCount; row++)
                    {
                        transposed[column, row] = arr[row, column];
                    }
                }
            }
            return transposed;
        }

        public double[,] AddArrays(double[,] arrayA, double[,] arrayB)
        {
            int rowCount = arrayA.GetLength(0);
            int columnCount = arrayA.GetLength(1);

            double[,] result = new double[rowCount, columnCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    result[i, j] = arrayA[i, j] + arrayB[i, j];
                }
            }
            return result;
        }

        public double[,] SubtractArrays(double[,] arrayA, double[,] arrayB)
        {
            int rowCount = arrayA.GetLength(0);
            int columnCount = arrayA.GetLength(1);

            double[,] result = new double[rowCount, columnCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    result[i, j] = arrayA[i, j] - arrayB[i, j];
                }
            }
            return result;
        }

        public double[,] DivideArrayByValue(double[,] arrayA, double value)
        {
            int rowCount = arrayA.GetLength(0);
            int columnCount = arrayA.GetLength(1);

            double[,] result = new double[rowCount, columnCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    result[i, j] = arrayA[i, j] / value;
                }
            }
            return result;
        }

        public double[,] MultiplyArrays(double[,] arrayA, double[,] arrayB)
        {
            int rowCount = arrayA.GetLength(0);
            int columnCount = arrayB.GetLength(1);
            double[,] result = new double[rowCount, columnCount];

            if (arrayA.GetLength(1) != arrayB.GetLength(0))
            {
                Console.WriteLine("Multiplication of array not possible, because they have incompatible sizes");
                return result;
            }
            
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    double v = 0;
                    for (int k = 0; k < arrayA.GetLength(1); k++)
                    {
                        v += arrayA[i, k] * arrayB[k, j];
                    }
                    result[i, j] = v;
                }
            }
          return result;
        }
    }
}
