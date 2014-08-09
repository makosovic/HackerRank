using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship1p
{
    class Solution
    {
        public static Coordinates T = new Coordinates();
        public static int Size { get; set; }
        public static bool HitFound { get; set; }

        public static List<Coordinates> HitList = new List<Coordinates>();

        static void Main(String[] args)
        {
            /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */

            var board = GetInput();

            if (HitFound)
            {
                if (HitList.Count == 1)
                {
                    OutputAround(board, HitList.ElementAt(0));
                }
                else if (HitList.ElementAt(0).Row == HitList.ElementAt(1).Row)
                {
                    if (HitList.ElementAt(0).Col > 0 && board[HitList.ElementAt(0).Row][HitList.ElementAt(0).Col - 1] == '-') Console.WriteLine(HitList.ElementAt(0).Row + " " + (HitList.ElementAt(0).Col - 1));
                    else if ((HitList.ElementAt(HitList.Count() - 1).Col < (Size - 1)) && board[HitList.ElementAt(0).Row][HitList.ElementAt(HitList.Count() - 1).Col + 1] == '-') Console.WriteLine(HitList.ElementAt(HitList.Count() - 1).Row + " " + (HitList.ElementAt(HitList.Count() - 1).Col + 1));
                }
                else if (HitList.ElementAt(0).Col == HitList.ElementAt(1).Col)
                {
                    if (HitList.ElementAt(0).Row > 0 && board[HitList.ElementAt(0).Row - 1][HitList.ElementAt(0).Col] == '-') Console.WriteLine((HitList.ElementAt(0).Row - 1) + " " + HitList.ElementAt(0).Col);
                    else if ((HitList.ElementAt(HitList.Count() - 1).Row < (Size - 1)) && board[HitList.ElementAt(HitList.Count() - 1).Row + 1][HitList.ElementAt(0).Col] == '-') Console.WriteLine((HitList.ElementAt(HitList.Count() - 1).Row + 1) + " " + HitList.ElementAt(HitList.Count() - 1).Col);
                }
            }
            else
            {
                RandomizeT(board);
                Output();
            }
        }

        private static void RandomizeT(string[] board)
        {
            var rng = new Random();
            T.Row = rng.Next(0, Size);
            T.Col = rng.Next(0, Size);

            while (board[T.Row][T.Col] != '-')
            {
                T.Row = rng.Next(0, Size);
                T.Col = rng.Next(0, Size);
            }
        }

        private static void Output()
        {
            Console.WriteLine(T.Row + " " + T.Col);
        }

        private static bool OutputAround(string[] board, Coordinates x)
        {
            if (x.Row > 0)
            {
                if (board[x.Row - 1][x.Col] == '-')
                {
                    Console.WriteLine((x.Row - 1) + " " + x.Col);
                    return true;
                }
            }
            if (x.Col < Size - 1)
            {
                if (board[x.Row][x.Col + 1] == '-')
                {
                    Console.WriteLine(x.Row + " " + (x.Col + 1));
                    return true;
                }
            }
            if (x.Row < Size - 1)
            {
                if (board[x.Row + 1][x.Col] == '-')
                {
                    Console.WriteLine((x.Row + 1) + " " + x.Col);
                    return true;
                }
            }
            if (x.Col > 0)
            {
                if (board[x.Row][x.Col - 1] == '-')
                {
                    Console.WriteLine(x.Row + " " + (x.Col - 1));
                    return true;
                }
            }
            return false;
        }

        private static string[] GetInput()
        {
            //get first line
            var line = Console.ReadLine();
            if (line != null) Size = Int32.Parse(line);

            //get the rest of input
            var board = new string[Size];
            for (var i = 0; i < Size; i++)
            {
                var tmpLine = Console.ReadLine();
                board[i] = tmpLine;

                if (tmpLine != null)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        if (tmpLine[j] == 'h')
                        {
                            HitFound = true;
                            var hit = new Coordinates { Row = i, Col = j };
                            HitList.Add(hit);
                        }
                    }
                }
            }
            return board;
        }





        internal class Coordinates
        {
            public Coordinates(int row, int col)
            {
                Row = row;
                Col = col;
            }

            public Coordinates()
            {
                Row = 0;
                Col = 0;
            }


            public int Row { get; set; }
            public int Col { get; set; }
        }
    }
}