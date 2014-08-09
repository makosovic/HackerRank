using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Play
{
    class PlayGame
    {
        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            var n = Play();
            watch.Stop();
            var elapsedS = watch.ElapsedMilliseconds / 1000;
            var nps = n/elapsedS;

            Console.WriteLine("\n\nYou lost the game! AI made {0} moves, with average {1} moves per second!", n, nps);

            Console.ReadLine();
        }

        public static int Play()
        {
            int[,] board = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            bool gameStartFlag = true;
            var noMoves = 0;

            while (true)
            {
                if (gameStartFlag)
                {
                    InitBoard(board);
                    gameStartFlag = false;
                }

                for (int i = 0; i < 4; i++)
                {
                    Console.WriteLine(board[i,0] + " " + board[i,1] + " " + board[i,2] + " " + board[i,3]);
                }

                var watch = Stopwatch.StartNew();

                var move = Solution.NextMove(board);
                noMoves++;
                Console.WriteLine("\n" + move);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                Console.WriteLine("Move took " + elapsedMs + "ms to compute.\n");

                var newboard = (int[,])board.Clone();
                Solution.ExecuteMove(move, newboard);

                if (Solution.BoardsAreTheSame(newboard, board)) break;

                board = (int[,])AddRandTile(newboard).Clone();
            }

            return noMoves;
        }

        private static void InitBoard(int[,] board)
        {
            Random rnd = new Random();

            int x1 = rnd.Next(0, 4);
            int y1 = rnd.Next(0, 4);

            int z1 = rnd.Next(0, 10);
            if (z1 == 0) board[x1, y1] = 4;
            else board[x1, y1] = 2;

            int x2, y2;

            do
            {
                x2 = rnd.Next(0, 4);
                y2 = rnd.Next(0, 4);
            } while (x2 == x1 || y2 == y1);
            
            int z2 = rnd.Next(0, 10);
            if (z2 == 0) board[x2, y2] = 4;
            else board[x2, y2] = 2;
        }

        private static int[,] AddRandTile(int[,] board)
        {
            int x, y;
            Random rnd = new Random();
            do
            {
                x = rnd.Next(0, 4);
                y = rnd.Next(0, 4);
            } while (board[x, y] != 0);

            int z = rnd.Next(0, 10);

            if (z == 0) board[x, y] = 4;
            else board[x, y] = 2;

            return board;
        }
    }
}
