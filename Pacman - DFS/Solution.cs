using System;
using System.Collections.Generic;

namespace Pacman___DFS
{

    class Solution
    {
        /* Head ends here */
        static void next_move(IList<string> board)
        {
            // When theres an exit
            if (board[1][0] == 'e')
                Console.WriteLine("LEFT");
            else if (board[2][1] == 'e')
                Console.WriteLine("DOWN");
            else if (board[1][2] == 'e')
                Console.WriteLine("RIGHT");
            else if (board[0][1] == 'e')
                Console.WriteLine("UP");

            // When theres a wall L, D, R or U
            else if (board[1][0] == '#')
            {
                if (board[2][1] != '#')
                    Console.WriteLine("DOWN");
                else if (board[1][2] != '#')
                    Console.WriteLine("RIGHT");
                else 
                    Console.WriteLine("UP");
            }
            else if (board[2][1] == '#')
            {
                if (board[1][2] != '#')
                    Console.WriteLine("RIGHT");
                else if (board[0][1] != '#')
                    Console.WriteLine("UP");
                else
                    Console.WriteLine("LEFT");
            }
            else if (board[1][2] == '#')
            {
                if (board[0][1] != '#')
                    Console.WriteLine("UP");
                else if (board[1][1] != '#')
                    Console.WriteLine("LEFT");
                else
                    Console.WriteLine("DOWN");
            }
            else if (board[0][1] == '#')
            {
                if (board[1][0] != '#')
                    Console.WriteLine("LEFT");
                else if (board[2][1] != '#')
                    Console.WriteLine("DOWN");
                else
                    Console.WriteLine("RIGTH");
            }

            // When theres a wall in the corner
            else if (board[0][0] == '#')
                Console.WriteLine("LEFT");
            else if (board[2][0] == '#')
                Console.WriteLine("LEFT");
            else if (board[2][2] == '#')
                Console.WriteLine("RIGHT");
            else if (board[2][0] == '#')
                Console.WriteLine("RIGHT");

            // When theres no walls
            else
                Console.WriteLine("DOWN");
        }
        /* Tail starts here */
        static void Main(String[] args)
        {
            //Game
            var player = Console.ReadLine();
            var board = new String[3];
            for (var i = 0; i < 3; i++)
                board[i] = Console.ReadLine();
            next_move(board);
        }
    }
}