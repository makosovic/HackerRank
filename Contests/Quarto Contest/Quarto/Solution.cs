using System;
using System.Collections.Generic;

namespace Quarto
{
    public enum Action
    {
        Place,
        Pick
    }

    class Solution
    {
        static void Main(String[] args)
        {
            var board = new int[4, 4];
            var remaining = new List<int>();
            int? toBePlaced = null;
            var player = Convert.ToInt32(Console.ReadLine());
            var action = Console.ReadLine() == "PLACE" ? Action.Place : Action.Pick;

            for (var i = 0; i < 4; i++)
            {
                var line = Console.ReadLine().Split(' ');
                for (var j = 0; j < 4; j++)
                    board[i, j] = Convert.ToInt32(line[j]);
            }

            var remainingN = Convert.ToInt32(Console.ReadLine());
            for (var i = 0; i < remainingN; i++)
            {
                remaining.Add(Convert.ToInt32(Console.ReadLine()));
            }

            if (action == Action.Place) toBePlaced = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine(NextMove(player, action, board, remaining, toBePlaced));
        }

        private static string NextMove(int player, Action action, int[,] board, IEnumerable<int> remaining, int? toBePlaced)
        {
            if (action == Action.Place && toBePlaced != null) return FindMovePlace(board, (int)toBePlaced);
            return FindMovePick(board, remaining);
        }

        private static string FindMovePlace(int[,] board, int toBePlaced)
        {
            var iLost = "";
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    if (board[row, col] == -1)
                    {
                        if (IsWinningMove(toBePlaced, board, row, col)) return row + " " + col;
                        iLost = row + " " + col;
                    }
                }
            }
            return iLost;
        }

        private static string FindMovePick(int[,] board, IEnumerable<int> remaining)
        {
            var iLost = 0;
            foreach (var item in remaining)
            {
                for (var row = 0; row < 4; row++)
                    for (var col = 0; col < 4; col++)
                        if (!IsWinningMove(item, board, row, col)) return item.ToString();

                iLost = item;
            }
            return iLost.ToString();
        }

        private static bool IsWinningMove(int value, int[,] board, int row, int col)
        {
            board[row, col] = value;
            var transBoard = (int[,])board.Clone();
            transBoard = TransposeBoard(transBoard);

            var isWinningMove = CheckRows(board);
            isWinningMove = isWinningMove || CheckRows(transBoard);
            isWinningMove = isWinningMove || CheckDiagonals(board);

            return isWinningMove;
        }

        private static bool CheckRows(int[,] board)
        {
            var isWinningMove = false;

            for (var i = 0; i < 4; i++)
            {
                var row = new[]
                {
                    ConvertToBinary(board[i, 0]),
                    ConvertToBinary(board[i, 1]),
                    ConvertToBinary(board[i, 2]),
                    ConvertToBinary(board[i, 3])
                };
                isWinningMove = isWinningMove || CheckRow(row);
            }
            return isWinningMove;
        }

        private static bool CheckDiagonals(int[,] board)
        {
            var diag1 = new[]
            {
                ConvertToBinary(board[0, 0]),
                ConvertToBinary(board[1, 1]),
                ConvertToBinary(board[2, 2]),
                ConvertToBinary(board[3, 3])
            };

            var isWinningMove = CheckRow(diag1);

            var diag2 = new[]
            {
                ConvertToBinary(board[0, 3]),
                ConvertToBinary(board[1, 2]),
                ConvertToBinary(board[2, 1]),
                ConvertToBinary(board[3, 0])
            };

            isWinningMove = isWinningMove || CheckRow(diag2);

            return isWinningMove;
        }

        private static bool CheckRow(string[] row)
        {
            if (row[0] == "XXXX" || row[1] == "XXXX" || row[2] == "XXXX" || row[3] == "XXXX") return false;
            for (var i = 0; i < 4; i++)
                if (!(row[0][i] == row[1][i] && row[0][i] == row[2][i] && row[0][i] == row[3][i])) return false;
            return true;
        }

        private static string ConvertToBinary(int value)
        {
            switch (value)
            {
                case -1:
                    return "XXXX";
                case 0:
                    return "0000";
                case 1:
                    return "0001";
                case 2:
                    return "0010";
                case 3:
                    return "0011";
                case 4:
                    return "0100";
                case 5:
                    return "0101";
                case 6:
                    return "0110";
                case 7:
                    return "0111";
                case 8:
                    return "1000";
                case 9:
                    return "1001";
                case 10:
                    return "1010";
                case 11:
                    return "1011";
                case 12:
                    return "1100";
                case 13:
                    return "1101";
                case 14:
                    return "1110";
                case 15:
                    return "1111";
                default:
                    return "XXXX";
            }
        }

        private static int[,] TransposeBoard(int[,] board)
        {
            var result = new int[4, 4];

            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    result[i, j] = board[j, i];

            return result;
        }
    }
}
