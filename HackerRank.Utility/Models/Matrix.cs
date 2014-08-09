using System;

namespace HackerRank.Utility.Models
{

    public class Matrix
    {

        #region properties

        public double this[int i, int j]
        {
            get { return _matrix[i][j]; }
            set { _matrix[i][j] = value; }
        }

        public double[] this[int i]
        {
            get { return _matrix[i]; }
            set { _matrix[i] = value; }
        }

        public int RowCount
        {
            get { return _matrix.Length; }
        }

        public int ColumnCount
        {
            get { return _matrix[0].Length; }
        }

        public double[][] Value
        {
            get { return _matrix; }
        }

        #endregion

        #region fields

        private double[][] _matrix;

        #endregion

        #region constructors

        public Matrix(int m)
        {
            _matrix = new double[m][];
        }

        public Matrix(int m, int n)
        {
            _matrix = new double[m][];
        }

        public Matrix(double[][] matrix)
        {
            _matrix = matrix;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates matrix inverse
        /// </summary>
        /// <returns>Matrix</returns>
        public Matrix Invert()
        {
            int n = _matrix.Length;
            int[] perm;
            int toggle;

            var matrix = Duplicate(_matrix);

            double[][] lum = MatrixDecompose(matrix, out perm, out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            var b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }
                double[] x = HelperSolve(lum, b);
                for (int j = 0; j < n; ++j)
                    _matrix[j][i] = x[j];
            }
            return this;
        }

        /// <summary>
        /// Transposes a matrix
        /// </summary>
        /// <returns>Matrix</returns>
        public Matrix Transpose()
        {
            var newMatrix = new double[_matrix[0].Length][];

            for (int i = 0; i < _matrix[0].Length; i++)
            {
                var newRow = new double[_matrix.Length];
                for (int j = 0; j < _matrix.Length; j++)
                {
                    newRow[j] = _matrix[j][i];
                }
                newMatrix[i] = newRow;
            }

            _matrix = newMatrix;
            return this;
        }

        /// <summary>
        /// Returns deep copy of a matrix
        /// </summary>
        /// <returns>Matrix</returns>
        public Matrix DeepCopy()
        {
            return new Matrix(Duplicate(_matrix));
        }

        #endregion

        #region private methods

        private double[][] Duplicate(double[][] matrix)
        {
            // assumes matrix is not null.
            double[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (int i = 0; i < matrix.Length; ++i) // copy the values
                for (int j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        private double[][] MatrixCreate(int rows, int cols)
        {
            // creates a matrix initialized to all 0.0s
            // do error checking here?
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols]; // auto init to 0.0
            return result;
        }

        private double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition.
            // assumes matrix is square.
            int n = matrix.Length; // convenience
            perm = new int[n];
            for (int i = 0; i < n; ++i) { perm[i] = i; }
            toggle = 1;
            for (int j = 0; j < n - 1; ++j) // each column
            {
                double colMax = Math.Abs(matrix[j][j]); // largest val in col j
                int pRow = j;
                for (int i = j + 1; i < n; ++i)
                {
                    if (matrix[i][j] > colMax)
                    {
                        colMax = matrix[i][j];
                        pRow = i;
                    }
                }
                if (pRow != j) // swap rows
                {
                    double[] rowPtr = matrix[pRow];
                    matrix[pRow] = matrix[j];
                    matrix[j] = rowPtr;
                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;
                    toggle = -toggle; // row-swap toggle
                }
                if (Math.Abs(matrix[j][j]) < 1.0E-20)
                    return null; // consider a throw
                for (int i = j + 1; i < n; ++i)
                {
                    matrix[i][j] /= matrix[j][j];
                    for (int k = j + 1; k < n; ++k)
                        matrix[i][k] -= matrix[i][j] * matrix[j][k];
                }
            } // main j column loop
            return matrix;
        }

        private double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // solve luMatrix * x = b
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);
            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }
            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }
            return x;
        }

        #endregion

    }

}
