using System;
using System.Collections.Generic;
using System.Linq;

namespace BotCleanLarge
{
    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    class Node
    {
        public Position Position { get; set; }
        public int Distance { get; set; }
    }

    class Solution
    {
        /* Head ends here */
        static void next_move(int posr, int posc, int dimh, int dimw, String[] board)
        {
            var bot = new Position() { X = posr, Y = posc };
            var dirty = new List<Position>();
            var visited = new List<Position>();
            var nextPosition = new Position();
            var best = Int32.MaxValue;

            for (var i = 0; i < dimh; i++)
                for (var j = 0; j < dimw; j++)
                    if (board[i][j] == 'd') dirty.Add(new Position() { X = i, Y = j });

            foreach (var item in dirty)
            {
                var distance = GetDistance(bot, item) + GetChildDistance(item, dirty, visited);

                if (distance < best)
                {
                    best = distance;
                    nextPosition = item;
                }

                visited.Clear();
            }

            var gor = nextPosition.X;
            var goc = nextPosition.Y;

            if (board[posr][posc] == 'd')
                Console.WriteLine("CLEAN");
            else
            {
                if (goc == posc && gor < posr) Console.WriteLine("UP");
                if (goc == posc && gor > posr) Console.WriteLine("DOWN");
                if (goc < posc) Console.WriteLine("LEFT");
                if (goc > posc) Console.WriteLine("RIGHT");
            }
        }

        private static int GetChildDistance(Position position, List<Position> dirty, List<Position> visited)
        {
            visited.Add(position);
            var best = Int32.MaxValue;

            var closestDirty = Top5Dirty(position, dirty);

            foreach (var item in closestDirty)
            {
                if (visited.Contains(item.Position)) continue;
                var distance = item.Distance;

                if (visited.Count < 5) distance += GetChildDistance(item.Position, dirty, visited);
                visited.Remove(item.Position);
                if (distance < best) best = distance;
            }
            return best == Int32.MaxValue ? 0 : best;
        }

        private static IEnumerable<Node> Top5Dirty(Position position, IEnumerable<Position> dirty)
        {
            var list = dirty.Select(item => new Node() { Position = item, Distance = GetDistance(position, item) }).ToList();
            list.Sort(CompareDistance);

            return list.Skip(1).Take(5);
        }

        private static int CompareDistance(Node x, Node y)
        {
            return x.Distance.CompareTo(y.Distance);
        }

        private static int GetDistance(Position pos1, Position pos2)
        {
            return Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);
        }

        /* Tail starts here */
        static void Main(String[] args)
        {
            String temp = Console.ReadLine();
            String[] position = temp.Split(' ');
            int[] pos = new int[2];
            for (int i = 0; i < 2; i++) pos[i] = Convert.ToInt32(position[i]);
            String[] dimension = Console.ReadLine().Split(' ');
            int[] dim = new int[2];
            for (int i = 0; i < 2; i++) dim[i] = Convert.ToInt32(dimension[i]);
            String[] board = new String[dim[0]];
            for (int i = 0; i < dim[0]; i++)
            {
                board[i] = Console.ReadLine();
            }
            next_move(pos[0], pos[1], dim[0], dim[1], board);
        }
    }
}