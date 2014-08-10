using System;
using System.Globalization;

namespace CharlieOfficeSpacePrices
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
            _matrix = MatrixCreate(m, n);
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

    public static class MatrixAlgebra
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

    #region Algebra class

    public static class Algebra
    {
        /// <summary>
        /// Calculates n factorial
        /// </summary>
        /// <param name="n"></param>
        /// <returns>int</returns>
        public static int Factorial(int n)
        {
            if (n <= 1)
                return 1;
            return n*Factorial(n - 1);
        }
    }

    #endregion

    #region Combinatorics class

    public static class Combinatorics
    {
        /// <summary>
        /// Returns the number of combinations of n things taken k at a time without repetition
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int Combinations(int n, int k)
        {
            return Algebra.Factorial(n) / (Algebra.Factorial(k) * Algebra.Factorial(n - k));
        }
    }

    #endregion

    #region PolynomialRegression class

    public static class PolynomialRegression
    {
        #region fields

        private static readonly int[] TermsCount = { 0, 3, 6, 11, 20, 32 };

        private static Matrix _dataVariables;
        private static Vector _dataResponse;

        private static Matrix _equationsMatrix;
        private static Vector _rhsVector;

        #endregion

        #region public methods

        public static Vector CalculateRegression(Matrix dataVariables, Vector dataResponse, Matrix queryVariables)
        {
            int variablesCount = queryVariables.ColumnCount;

            Vector coeficients = CalculateRegressionCoeficients(dataVariables, dataResponse);
            Vector queryResult = new Vector(queryVariables.RowCount);

            for (int i = 0; i < queryVariables.RowCount; i++)
            {
                queryResult[i] = coeficients[0];

                for (int j = 1; j < variablesCount + 1; j++)
                {
                    queryResult[i] += coeficients[j]*queryVariables[i][j - 1];
                    queryResult[i] += coeficients[j + variablesCount] * queryVariables[i][j - 1];
                }

                queryResult[i] += AllVariableInteractionSums(coeficients, queryVariables[i]);
            }

            return queryResult;
        }

        #endregion

        #region private methods

        private static Vector CalculateRegressionCoeficients(Matrix variables, Vector response)
        {
            _dataVariables = variables;
            _dataResponse = response;

            InitEquations(variables.RowCount, variables.ColumnCount);

            _equationsMatrix.Invert();

            return MatrixAlgebra.MatrixVectorProduct(_equationsMatrix, _rhsVector);
        }

        private static void InitEquations(int dataRows, int variablesCount)
        {
            _equationsMatrix = new Matrix(TermsCount[variablesCount], TermsCount[variablesCount]);
            _rhsVector = new Vector(TermsCount[variablesCount]);

            _equationsMatrix[0][0] = dataRows;

            // calculate response sum
            double y = 0;
            for (int i = 0; i < dataRows; i++)
            {
                y += _dataResponse[i];
            }
            _rhsVector[0] = y;

            double allVariablesInteraction = 0;

            // calculate first term
            for (int i = 0; i < variablesCount; i++)
            {
                double firstTermVariableSum = 0;

                for (int j = 0; j < dataRows; j++)
                {
                    firstTermVariableSum += _dataVariables[j][i];
                }

                _equationsMatrix[0][i + 1] = firstTermVariableSum;
                _equationsMatrix[i + 1][0] = firstTermVariableSum;
                _rhsVector[i + 1] = firstTermVariableSum * _rhsVector[0];

                // cumulative product for interaction of all variables
                allVariablesInteraction *= firstTermVariableSum;

                // set second term
                double secondTermVariableSum = Math.Pow(firstTermVariableSum, 2);

                _equationsMatrix[0][i + 1 + variablesCount] = secondTermVariableSum;
                _equationsMatrix[i + 1 + variablesCount][0] = secondTermVariableSum;
                _rhsVector[i + 1 + variablesCount] = secondTermVariableSum * _rhsVector[0];
            }

            // calculate interaction terms
            if (variablesCount > 1)
            {
                for (int i = 1; i < variablesCount; i++)
                {
                    for (int j = i + 1; j < variablesCount + 1; j++)
                    {
                        double twoVariablesInteraction = _equationsMatrix[0][i] * _equationsMatrix[0][j];
                        _equationsMatrix[0][i + 2 * variablesCount] = twoVariablesInteraction;
                        _equationsMatrix[i + 2 * variablesCount][0] = twoVariablesInteraction;
                        _rhsVector[i + 2 * variablesCount] = twoVariablesInteraction * _rhsVector[0];

                        int twoVariablesInteractionCount = Combinatorics.Combinations(variablesCount, 2);

                        for (int k = j + 1; k < variablesCount + 1; k++)
                        {
                            double threeVariablesInteraction = _equationsMatrix[0][i] * _equationsMatrix[0][j] * _equationsMatrix[0][k];
                            _equationsMatrix[0][i + 2 * variablesCount + twoVariablesInteractionCount] = threeVariablesInteraction;
                            _equationsMatrix[i + 2 * variablesCount + twoVariablesInteractionCount][0] = threeVariablesInteraction;
                            _rhsVector[i + 2 * variablesCount + twoVariablesInteractionCount] = threeVariablesInteraction * _rhsVector[0];

                            int threeVariablesInteractionCount = Combinatorics.Combinations(variablesCount, 3);

                            for (int l = k + 1; l < variablesCount + 1; l++)
                            {
                                double fourVariablesInteraction = _equationsMatrix[0][i] * _equationsMatrix[0][j] * _equationsMatrix[0][k] * _equationsMatrix[0][l];
                                _equationsMatrix[0][i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount] = fourVariablesInteraction;
                                _equationsMatrix[i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount][0] = fourVariablesInteraction;
                                _rhsVector[i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount] = fourVariablesInteraction * _rhsVector[0];

                                int fourVariablesInteractionCount = Combinatorics.Combinations(variablesCount, 4);

                                for (int m = l + 1; m < variablesCount + 1; m++)
                                {
                                    double fiveVariablesInteraction = _equationsMatrix[0][i] * _equationsMatrix[0][j] * _equationsMatrix[0][k] * _equationsMatrix[0][l] * _equationsMatrix[0][m];
                                    _equationsMatrix[0][i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount + fourVariablesInteractionCount] = fiveVariablesInteraction;
                                    _equationsMatrix[i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount + fourVariablesInteractionCount][0] = fiveVariablesInteraction;
                                    _rhsVector[i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount + fourVariablesInteractionCount] = fiveVariablesInteraction * _rhsVector[0];
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 1; i < TermsCount[variablesCount]; i++)
            {
                for (int j = 1; j < TermsCount[variablesCount]; j++)
                {
                    _equationsMatrix[i][j] = _equationsMatrix[0][i] * _equationsMatrix[j][0];
                }
            }
        }



        private static double AllVariableInteractionSums(Vector coeficients, double[] queryVariables)
        {
            int variablesCount = queryVariables.Length;

            double resultSum = 0;

            if (variablesCount > 1)
            {
                for (int i = 1; i < variablesCount; i++)
                {
                    for (int j = i + 1; j < variablesCount + 1; j++)
                    {
                        resultSum += coeficients[i + 2*variablesCount]*queryVariables[i - 1]*queryVariables[j - 1];

                        int twoVariablesInteractionCount = Combinatorics.Combinations(variablesCount, 2);

                        for (int k = j + 1; k < variablesCount + 1; k++)
                        {
                            resultSum += coeficients[i + 2 * variablesCount + twoVariablesInteractionCount] * queryVariables[i - 1] * queryVariables[j - 1] * queryVariables[k - 1];

                            int threeVariablesInteractionCount = Combinatorics.Combinations(variablesCount, 3);

                            for (int l = k + 1; l < variablesCount + 1; l++)
                            {
                                resultSum += coeficients[i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount] * queryVariables[i - 1] * queryVariables[j - 1] * queryVariables[k - 1] * queryVariables[l - 1];

                                int fourVariablesInteractionCount = Combinatorics.Combinations(variablesCount, 4);

                                for (int m = l + 1; m < variablesCount + 1; m++)
                                {
                                    resultSum += coeficients[i + 2 * variablesCount + twoVariablesInteractionCount + threeVariablesInteractionCount + fourVariablesInteractionCount] * queryVariables[i - 1] * queryVariables[j - 1] * queryVariables[k - 1] * queryVariables[l - 1] * queryVariables[m - 1];
                                }
                            }
                        }
                    }
                }
            }

            return resultSum;
        }

        #endregion

    }

    #endregion

    class Solution
    {
        private static void Main(String[] args)
        {
            //Vector priceVector;
            //Matrix variablesMatrix;
            //Matrix queryMatrix;

            //string firstLine = Console.ReadLine();
            //string[] parameters = firstLine.Split(' ');
            //int variablesCount = Convert.ToInt32(parameters[0]);
            //int observationsCount = Convert.ToInt32(parameters[1]);

            //priceVector = new Vector(observationsCount);
            //variablesMatrix = new Matrix(observationsCount, variablesCount);

            //for (int i = 0; i < observationsCount; i++)
            //{
            //    string line = Console.ReadLine();
            //    string[] lineData = line.Split(' ');
            //    double[] variablesMatrixRow = new double[variablesCount];

            //    for (int j = 0; j < variablesCount + 1; j++)
            //    {
            //        if (j < variablesCount)
            //            variablesMatrixRow[j] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
            //        else
            //            priceVector[i] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
            //    }

            //    variablesMatrix[i] = variablesMatrixRow;
            //}

            //int resultCount = Convert.ToInt32(Console.ReadLine());
            //queryMatrix = new Matrix(resultCount, variablesCount);

            //for (int i = 0; i < resultCount; i++)
            //{
            //    string line = Console.ReadLine();
            //    string[] lineData = line.Split(' ');
            //    double[] queryMatrixRow = new double[variablesCount];

            //    for (int j = 0; j < variablesCount; j++)
            //    {
            //        queryMatrixRow[j] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
            //    }

            //    queryMatrix[i] = queryMatrixRow;
            //}




            System.IO.StreamReader myFile = new System.IO.StreamReader("C:\\Users\\Marko\\Downloads\\charlie.txt");

            Vector priceVector;
            Matrix variablesMatrix;
            Matrix queryMatrix;

            string firstLine = myFile.ReadLine();
            string[] parameters = firstLine.Split(' ');
            int variablesCount = Convert.ToInt32(parameters[0]);
            int observationsCount = Convert.ToInt32(parameters[1]);

            priceVector = new Vector(observationsCount);
            variablesMatrix = new Matrix(observationsCount, variablesCount);

            for (int i = 0; i < observationsCount; i++)
            {
                string line = myFile.ReadLine();
                string[] lineData = line.Split(' ');
                double[] variablesMatrixRow = new double[variablesCount];

                for (int j = 0; j < variablesCount + 1; j++)
                {
                    if (j < variablesCount)
                        variablesMatrixRow[j] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
                    else
                        priceVector[i] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
                }

                variablesMatrix[i] = variablesMatrixRow;
            }

            int resultCount = Convert.ToInt32(myFile.ReadLine());
            queryMatrix = new Matrix(resultCount, variablesCount);

            for (int i = 0; i < resultCount; i++)
            {
                string line = myFile.ReadLine();
                string[] lineData = line.Split(' ');
                double[] queryMatrixRow = new double[variablesCount];

                for (int j = 0; j < variablesCount; j++)
                {
                    queryMatrixRow[j] = Double.Parse(lineData[j], CultureInfo.InvariantCulture);
                }

                queryMatrix[i] = queryMatrixRow;
            }

            myFile.Close();






            // Calculate price for result variables using calculated coeficients
            Vector estimatedPrices = PolynomialRegression.CalculateRegression(variablesMatrix, priceVector, queryMatrix);

            // Display the results

            for (int i = 0; i < estimatedPrices.Length; i++)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F}", estimatedPrices[i]));
            }





            Console.ReadLine();
        }
    }
}
