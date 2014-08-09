using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Click_o_Mania
{
    class Solution
    {

        private static readonly Dictionary<string, bool> GridHistory = new Dictionary<string, bool>(); 

        /* Head ends here */
        static void nextMove(int x, int y, int color, String[] grid)
        {
            //Your logic here

            var max = 0;
            var maxT = new Coordinates();

            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    var next = new Coordinates(i, j);

                    if (!WasIHere(next) && grid[next.X][next.Y] != '-')
                    {
                        var connected = CheckAround(next, next, x, y, grid[i][j], grid);

                        if (connected > max)
                        {
                            max = connected;
                            maxT.X = i;
                            maxT.Y = j;
                        }
                    }
                }
            }

            Console.WriteLine(maxT.X + " " + maxT.Y);
            Console.ReadLine();
        }

        static int CheckAround(Coordinates last, Coordinates target, int row, int col, char color, String[] grid)
        {
            MarkIWasHere(target);
            var connected = 0;
            if (target.X > 0)
            {
                var next = new Coordinates(target.X - 1, target.Y);
                if ((!((next.X == last.X) && (next.Y == last.Y))) && grid[target.X][target.Y] == grid[next.X][next.Y])
                {
                    connected++;
                    if(!WasIHere(next))
                    {
                        connected += CheckAround(target, next, row, col, grid[next.X][next.Y], grid);
                    }
                }
            }
            if (target.Y > 0)
            {
                var next = new Coordinates(target.X, target.Y - 1);
                if ((!((next.X == last.X) && (next.Y == last.Y))) && grid[target.X][target.Y] == grid[next.X][next.Y])
                {
                    connected++;
                    if (!WasIHere(next))
                    {
                        connected += CheckAround(target, next, row, col, grid[next.X][next.Y], grid);
                    }
                }
            }
            if (target.X < row - 1)
            {
                var next = new Coordinates(target.X + 1, target.Y);
                if ((!((next.X == last.X) && (next.Y == last.Y))) && grid[target.X][target.Y] == grid[next.X][next.Y])
                {
                    connected++;
                    if (!WasIHere(next))
                    {
                        connected += CheckAround(target, next, row, col, grid[next.X][next.Y], grid);
                    }
                }
            }
            if (target.Y < col - 1)
            {
                var next = new Coordinates(target.X, target.Y + 1);
                if ((!((next.X == last.X) && (next.Y == last.Y))) && grid[target.X][target.Y] == grid[next.X][next.Y])
                {
                    connected++;
                    if (!WasIHere(next))
                    {
                        connected += CheckAround(target, next, row, col, grid[next.X][next.Y], grid);
                    }
                }
            }
            return connected;
        }

        private static void MarkIWasHere(Coordinates T)
        {
            GridHistory.Add(string.Format("{0}{1}", T.X, T.Y), true);
        }

        static bool WasIHere(Coordinates T)
        {
            bool result;
            GridHistory.TryGetValue(string.Format("{0}{1}", T.X, T.Y), out result);
            return result;
        }

        /* Tail starts here */
        static void Main(String[] args)
        {
            int x, y, k;
            var temp = Console.ReadLine();
            var temp_split = temp.Split(' ');
            x = Convert.ToInt32(temp_split[0]);
            y = Convert.ToInt32(temp_split[1]);
            k = Convert.ToInt32(temp_split[2]);
            var grid = new String[x];
            for (var i = 0; i < x; i++)
            {
                grid[i] = Console.ReadLine();
            }

            nextMove(x, y, k, grid);
        }

        internal class Coordinates
        {
            public Coordinates(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Coordinates()
            {
                X = 0;
                Y = 0;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}