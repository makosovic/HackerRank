
using System;
using HackerRank.Utility.Models;

namespace HackerRank.Utility.Algebra
{
    static class MatrixAlgebra
    {
        /// <summary>
        /// Returns product of matrixA and matrixB
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns>Matrix</returns>
        public static Matrix MatrixProduct(double[][] matrixA, double[][] matrixB)
        {
            int aRows = matrixA.Length; int aCols = matrixA[0].Length;
            int bRows = matrixB.Length; int bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");
            double[][] result = MatrixCreate(aRows, bCols);
            for (int i = 0; i < aRows; ++i) // each row of A
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k)
                        result[i][j] += matrixA[i][k] * matrixB[k][j];
            return new Matrix(result);
        }

        /// <summary>
        /// Multiplies Matrix a with Vector b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Vector</returns>
        public static Vector MatrixVectorProduct(Matrix a, Vector b)
        {
            Vector result = new Vector(a.RowCount);

            for (int i = 0; i < a.RowCount; i++)
            {
                double dotProduct = 0;
                for (int k = 0; k < a.ColumnCount; k++)
                {
                    dotProduct += a[i][k] * b[k];
                }
                result[i] = dotProduct;
            }

            return result;
        }

        private static double[][] MatrixCreate(int rows, int cols)
        {
            // creates a matrix initialized to all 0.0s
            // do error checking here?
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols]; // auto init to 0.0
            return result;
        }
    }
}
