using System;
using System.Globalization;

namespace CharlieHousingPrices
{

    #region Matrix class

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

    #endregion

    #region Vector class

    public class Vector
    {
        public double this[int n]
        {
            get { return _vector[n]; }
            set { _vector[n] = value; }
        }

        public int Length { get { return _vector.Length; } }

        private double[] _vector;

        public Vector(int n)
        {
            _vector = new double[n];
        }

        public Vector(double[] vector)
        {
            _vector = vector;
        }

        public double[] Value()
        {
            return _vector;
        }
    }

    #endregion

    #region MatrixAlgebra class

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

    #endregion

    class Solution
    {
        private static void Main(String[] args)
        {
            //Vector priceVector;
            //Matrix designMatrix;
            //Matrix variablesMatrix;

            //string firstLine = Console.ReadLine();
            //string[] parameters = firstLine.Split(' ');
            //int variablesCount = Convert.ToInt32(parameters[0]);
            //int observationsCount = Convert.ToInt32(parameters[1]);

            //priceVector = new Vector(observationsCount);
            //designMatrix = new Matrix(observationsCount, variablesCount + 1);

            //for (int i = 0; i < observationsCount; i++)
            //{
            //    string line = Console.ReadLine();
            //    string[] lineData = line.Split(' ');
            //    double[] designMatrixRow = new double[variablesCount + 1];

            //    for (int j = 0; j < variablesCount + 1; j++)
            //    {
            //        if (j == 0) designMatrixRow[j] = 1;
            //        if (j < variablesCount)
            //            designMatrixRow[j + 1] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
            //        else
            //            priceVector[i] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
            //    }

            //    designMatrix[i] = designMatrixRow;
            //}

            //int resultCount = Convert.ToInt32(Console.ReadLine());
            //variablesMatrix = new Matrix(resultCount, variablesCount);

            //for (int i = 0; i < resultCount; i++)
            //{
            //    string line = Console.ReadLine();
            //    string[] lineData = line.Split(' ');
            //    double[] variablesMatrixRow = new double[variablesCount];

            //    for (int j = 0; j < variablesCount; j++)
            //    {
            //        variablesMatrixRow[j] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
            //    }

            //    variablesMatrix[i] = variablesMatrixRow;
            //}
            System.IO.StreamReader myFile = new System.IO.StreamReader("C:\\Users\\Marko\\Downloads\\charlie.txt");

            Vector priceVector;
            Matrix designMatrix;
            Matrix variablesMatrix;

            string firstLine = myFile.ReadLine();
            string[] parameters = firstLine.Split(' ');
            int variablesCount = Convert.ToInt32(parameters[0]);
            int observationsCount = Convert.ToInt32(parameters[1]);

            priceVector = new Vector(observationsCount);
            designMatrix = new Matrix(observationsCount, variablesCount + 1);

            for (int i = 0; i < observationsCount; i++)
            {
                string line = myFile.ReadLine();
                string[] lineData = line.Split(' ');
                double[] designMatrixRow = new double[variablesCount + 1];

                for (int j = 0; j < variablesCount + 1; j++)
                {
                    if (j == 0) designMatrixRow[j] = 1;
                    if (j < variablesCount)
                        designMatrixRow[j + 1] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
                    else
                        priceVector[i] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
                }

                designMatrix[i] = designMatrixRow;
            }

            int resultCount = Convert.ToInt32(myFile.ReadLine());
            variablesMatrix = new Matrix(resultCount, variablesCount);

            for (int i = 0; i < resultCount; i++)
            {
                string line = myFile.ReadLine();
                string[] lineData = line.Split(' ');
                double[] variablesMatrixRow = new double[variablesCount];

                for (int j = 0; j < variablesCount; j++)
                {
                    variablesMatrixRow[j] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
                }

                variablesMatrix[i] = variablesMatrixRow;
            }

            myFile.Close();






            // Calculate regression coeficients
            Vector regressionCoeficientsVector = CalculateRegression(designMatrix, priceVector);

            // Calculate price for result variables using calculated coeficients
            double[] results = new double[resultCount];

            for (int i = 0; i < resultCount; i++)
            {
                results[i] = regressionCoeficientsVector[0];

                for (int j = 0; j < variablesCount; j++)
                {
                    results[i] += regressionCoeficientsVector[j+1] * variablesMatrix[i][j];
                }
            }

            // Display the results

            for (int i = 0; i < resultCount; i++)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F}", results[i]));
            }
        }

        private static Vector CalculateRegression(Matrix designMatrix, Vector priceVector)
        {
            Matrix transposedMatrix = designMatrix.DeepCopy().Transpose();

            Matrix productMatrix = MatrixAlgebra.MatrixProduct(transposedMatrix.Value, designMatrix.Value);

            productMatrix.Invert();

            Vector productVector = MatrixAlgebra.MatrixVectorProduct(transposedMatrix, priceVector);

            Vector resultVector = MatrixAlgebra.MatrixVectorProduct(productMatrix, productVector);

            return resultVector;
        }
    }
}
