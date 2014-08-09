using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _2048.Test
{
    [TestClass]
    public class ExecuteMoveTest
    {
        [TestMethod]
        public void SimpleTestMethodUP()
        {
            int[,] board = new int[,] { { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 } };
            int[,] expected = new int[,] { { 4, 4, 4, 4 }, { 4, 4, 4, 4 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };

            int[,] actual = Solution.ExecuteMoveUP(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        [TestMethod]
        public void SimpleTestMethodDOWN()
        {
            int[,] board = new int[,] { { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 4, 4, 4, 4 }, { 4, 4, 4, 4 } };

            int[,] actual = Solution.ExecuteMoveDOWN(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        [TestMethod]
        public void SimpleTestMethodLEFT()
        {
            int[,] board = new int[,] { { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 } };
            int[,] expected = new int[,] { { 4, 4, 0, 0 }, { 4, 4, 0, 0 }, { 4, 4, 0, 0 }, { 4, 4, 0, 0 } };

            int[,] actual = Solution.ExecuteMoveLEFT(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        [TestMethod]
        public void SimpleTestMethodRIGHT()
        {
            int[,] board = new int[,] { { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 }, { 2, 2, 2, 2 } };
            int[,] expected = new int[,] { { 0, 0, 4, 4 }, { 0, 0, 4, 4 }, { 0, 0, 4, 4 }, { 0, 0, 4, 4 } };

            int[,] actual = Solution.ExecuteMoveRIGHT(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }



        [TestMethod]
        public void ComplexTestMethodUP()
        {
            int[,] board = new int[,] { { 0, 0, 2, 2 }, { 0, 0, 0, 8 }, { 0, 8, 32, 4 }, { 2, 8, 32, 64 } };
            int[,] expected = new int[,] { { 2, 16, 2, 2 }, { 0, 0, 64, 8 }, { 0, 0, 0, 4 }, { 0, 0, 0, 64 } };

            int[,] actual = Solution.ExecuteMoveUP(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        [TestMethod]
        public void ComplexTestMethodDOWN()
        {
            int[,] board = new int[,] { { 0, 0, 2, 2 }, { 0, 0, 0, 8 }, { 0, 8, 32, 4 }, { 2, 8, 32, 64 } };
            int[,] expected = new int[,] { { 0, 0, 0, 2 }, { 0, 0, 0, 8 }, { 0, 0, 2, 4 }, { 2, 16, 64, 64 } };

            int[,] actual = Solution.ExecuteMoveDOWN(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        [TestMethod]
        public void ComplexTestMethodLEFT()
        {
            int[,] board = new int[,] { { 0, 0, 2, 2 }, { 0, 0, 0, 8 }, { 0, 8, 32, 4 }, { 2, 8, 32, 64 } };
            int[,] expected = new int[,] { { 4, 0, 0, 0 }, { 8, 0, 0, 0 }, { 8, 32, 4, 0 }, { 2, 8, 32, 64 } };

            int[,] actual = Solution.ExecuteMoveLEFT(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }

        [TestMethod]
        public void ComplexTestMethodRIGHT()
        {
            int[,] board = new int[,] { { 0, 0, 2, 2 }, { 0, 0, 0, 8 }, { 0, 8, 32, 4 }, { 2, 8, 32, 64 } };
            int[,] expected = new int[,] { { 0, 0, 0, 4 }, { 0, 0, 0, 8 }, { 0, 8, 32, 4 }, { 2, 8, 32, 64 } };

            int[,] actual = Solution.ExecuteMoveRIGHT(board);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j]);
        }
    }
}
