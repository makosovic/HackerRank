using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    class Solution
    {
        public static Coordinates T = new Coordinates();
        public static Coordinates[] Ts =
        {
            new Coordinates(0, 0), 
            new Coordinates(4, 2),
            new Coordinates(6, 4), 
            new Coordinates(7, 4),
            new Coordinates(3, 7), 
            new Coordinates(3, 8),
            new Coordinates(7, 7), 
            new Coordinates(7, 8),
            new Coordinates(7, 9), 
            new Coordinates(1, 4),
            new Coordinates(2, 4), 
            new Coordinates(3, 4),
            new Coordinates(4, 4), 
            new Coordinates(4, 0),
            new Coordinates(5, 0), 
            new Coordinates(6, 0),
            new Coordinates(7, 0), 
            new Coordinates(8, 0)
        };
        public static int Size { get; set; }
        public static bool HitFound { get; set; }
        public static bool notHC { get; set; }

        public static List<Coordinates> HitList = new List<Coordinates>();

        static void Main(String[] args)
        {
            /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */

            var board = GetInput();

            if (board[0] == "INIT")
            {
                InitOutput();
            }

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
            else if (board[0] != "INIT")
            {
                CheckHC(board);

                if (notHC)
                {
                    RandomizeT(board);
                    Output();
                }
                else
                {
                    NextT(board);
                    Output();
                }
            }
        }

        private static void NextT(string[] board)
        {
            var i = 0;
            T.Row = Ts[i].Row;
            T.Col = Ts[i].Col;
            i++;

            while (board[T.Row][T.Col] != '-')
            {
                T.Row = Ts[i].Row;
                T.Col = Ts[i].Col;
                i++;
            }
        }

        private static void CheckHC(string[] board)
        {
            foreach (var line in board)
            {
                foreach (var letter in line)
                {
                    if (letter != 'd' && letter != 'h' && letter != '-') notHC = true;
                }
            }
        }

        private static void InitOutput()
        {
            Console.WriteLine("9 9");
            Console.WriteLine("5 7");
            Console.WriteLine("3 5:2 5");
            Console.WriteLine("6 2:6 1");
            Console.WriteLine("2 2:2 0");
            Console.WriteLine("8 5:5 5");
            Console.WriteLine("5 9:1 9");
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
            if (line == "INIT") return new[] { "INIT" };
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