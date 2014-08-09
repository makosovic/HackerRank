using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BotCleanPartiallyObservable
{
    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

    }

    class Solution
    {
        /* Head ends here */
        static void next_move(int posr, int posc, IList<string> board, ICollection<Position> explored, ICollection<Position> unexplored, ICollection<Position> dirty)
        {
            var minl = 5 * 5;

            int gor = 0, goc = 0;
            var wander = true;
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    if (board[i][j] == 'd')
                    {
                        wander = false;

                        if (((Math.Abs(i - posr) + Math.Abs(j - posc)) >= minl) && !dirty.Any(x => x.X == i && x.Y == j))
                            dirty.Add(new Position(i, j));

                        if (((Math.Abs(i - posr) + Math.Abs(j - posc)) < minl))
                        {

                            if (!dirty.Any(x => x.X == i && x.Y == j)) dirty.Add(new Position(i, j));
                            minl = (Math.Abs(i - posr) + Math.Abs(j - posc));
                            gor = i;
                            goc = j;
                        }
                    }

                    if (board[i][j] != 'o' && !explored.Any(x => x.X == i && x.Y == j)) 
                        explored.Add(new Position(i, j));

                    if (!explored.Any(x => x.X == i && x.Y == j)) 
                        unexplored.Add(new Position(i, j));
                }
            }

            if (wander)
            {
                if (dirty.Count != 0)
                {
                    gor = dirty.First().X;
                    goc = dirty.First().Y;
                }
                else if (unexplored.Count != 0)
                {
                    gor = unexplored.First().X;
                    goc = unexplored.First().Y;
                }
            }

            //Make a move
            if (board[posr][posc] == 'd')
            {
                if (dirty.Any(x => x.X == posr && x.Y == posc)) 
                    dirty.Remove(dirty.First(x => x.X == posr && x.Y == posc));

                WriteState(explored, dirty);
                Console.WriteLine("CLEAN");
            }
            else
            {
                WriteState(explored, dirty);

                if (gor == posr && goc < posc) Console.WriteLine("LEFT");
                if (gor == posr && goc > posc) Console.WriteLine("RIGHT");
                if (gor < posr) Console.WriteLine("UP");
                if (gor > posr) Console.WriteLine("DOWN");
            }
        }
        /* Tail starts here */
        static void Main(String[] args)
        {
            //Collections
            var dirty = new List<Position>();
            var explored = new List<Position>();
            var unexplored = new List<Position>();

            //State
            ReadState(explored, dirty);

            //Game
            var temp = Console.ReadLine();
            var position = temp.Split(' ');
            var pos = new int[2];
            var board = new String[5];
            for (var i = 0; i < 5; i++)
                board[i] = Console.ReadLine();
            for (var i = 0; i < 2; i++) 
                pos[i] = Convert.ToInt32(position[i]);
            next_move(pos[0], pos[1], board, explored, unexplored, dirty);
        }

        static void ReadState(ICollection<Position> explored, ICollection<Position> dirty)
        {
            const string fileName = "myfile.txt";
            var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
            var sr = new StreamReader(file);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line != null)
                {
                    var lineDetails = line.Split(' ');
                    switch (lineDetails[0])
                    {
                        case "-":
                            explored.Add(new Position(Convert.ToInt32(lineDetails[1]), Convert.ToInt32(lineDetails[2])));
                            break;
                        case "d":
                            dirty.Add(new Position(Convert.ToInt32(lineDetails[1]), Convert.ToInt32(lineDetails[2])));
                            break;
                    }
                }
            }
            file.Close();
        }

        static void WriteState(IEnumerable<Position> explored, IEnumerable<Position> dirty)
        {
            //Write collections to file
            const string fileName = "myfile.txt";
            var file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var sw = new StreamWriter(file);
            foreach (var position in explored)
            {
                sw.WriteLine("- {0} {1}", position.X, position.Y);
            }
            foreach (var position in dirty)
            {
                sw.WriteLine("d {0} {1}", position.X, position.Y);
            }
            sw.Close();
        }
    }
}