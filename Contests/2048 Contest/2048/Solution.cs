using System;
using System.Collections.Generic;
using System.Linq;

namespace _2048
{
    public class Solution
    {
        public static float CprobThreshhold { get { return 0.0001f; } }
        public static int CacheDepthLimit { get { return 6; } }
        public static int SearchDepthLimit { get { return 6; } }

        internal class State
        {
            public State()
            {
                CprobThreshold = MaxDepth = CurrentDepth = CacheHits = MovesEvaluated = 0;
            }
            public Dictionary<int[,], float> BoardResultCacheDictionary = new Dictionary<int[,], float>();
            public float CprobThreshold { get; set; }
            public int MaxDepth { get; set; }
            public int CurrentDepth { get; set; }
            public int CacheHits { get; set; }
            public int MovesEvaluated { get; set; }
        }

        public static string[] Move =
        {
            "UP", 
            "DOWN", 
            "LEFT",
            "RIGHT"
        };

        public static String NextMove(int[,] board)
        {
            State state = new State();
            float best = 0;
            string bestMove = "";

            foreach (var move in Move.AsEnumerable())
            {
                state.CurrentDepth = 0;
                float result = ScoreTopLevelMove(state, move, board);

                if (result > best)
                {
                    best = result;
                    bestMove = move;
                }
            }
            return bestMove;
        }

        private static float ScoreTopLevelMove(State state, string move, int[,] board)
        {
            int[,] newBoard = (int[,])board.Clone();

            if (BoardsAreTheSame(ExecuteMove(move, newBoard), board)) return 0.0f;

            state.CprobThreshold = CprobThreshhold;

            return ScoreTileChooseNode(state, newBoard, 1.0f);
        }


        private static float ScoreTileChooseNode(State state, int[,] board, float cprob)
        {
            float result = 0;
            int numOpen = 0;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (board[i, j] == 0) numOpen++;

            cprob /= numOpen;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (board[i, j] == 0)
                    {
                        board[i, j] = 2;
                        result += ScoreMoveNode(state, board, cprob * 0.9f) * 0.9f;
                        board[i, j] = 4;
                        result += ScoreMoveNode(state, board, cprob * 0.1f) * 0.1f;
                    }

            return result / numOpen;
        }

        private static float ScoreMoveNode(State state, int[,] board, float cprob)
        {
            if (cprob < state.CprobThreshold || state.CurrentDepth >= CacheDepthLimit)
            {
                var transBoard = (int[,])board.Clone();
                transBoard = TransposeBoard(transBoard);
                if (state.CurrentDepth > state.MaxDepth)
                    state.MaxDepth = state.CurrentDepth;
                return (ScoreHeurBoard(board) + ScoreHeurBoard(transBoard));
            }

            if (state.CurrentDepth < CacheDepthLimit)
            {
                float bestCached;
                state.BoardResultCacheDictionary.TryGetValue(board, out bestCached);
                if (bestCached != 0.0f)
                {
                    state.CacheHits++;
                    return bestCached;
                }
            }

            float best = 0;

            state.CurrentDepth++;
            foreach (var move in Move.AsEnumerable())
            {
                int[,] newBoard = (int[,])board.Clone();

                state.MovesEvaluated++;
                if (BoardsAreTheSame(ExecuteMove(move, newBoard), board)) continue;

                float result = ScoreTileChooseNode(state, newBoard, cprob);
                if (result > best)
                    best = result;
            }

            state.CurrentDepth--;

            if (state.CurrentDepth < CacheDepthLimit)
            {
                if (!state.BoardResultCacheDictionary.ContainsKey(board)) state.BoardResultCacheDictionary.Add(board, best);
            }

            return best;
        }

        private static float ScoreHeurBoard(int[,] board)
        {
            float result = 0;
            int boardMaxiX = 0;
            int boardMaxiY = 0;

            for (int i = 0; i < 4; i++)
            {
                // if the tiles are empty
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0) result += 100000;
                }

                int maxi = 0;
                int maxrank = 0;

                for (int j = 0; j < 4; j++)
                {
                    int rank = board[i, j];

                    if (rank > maxrank)
                    {
                        maxrank = rank;
                        maxi = j;
                    }
                }

                if (board[i, maxi] > board[boardMaxiX, boardMaxiY])
                {
                    boardMaxiX = i;
                    boardMaxiY = maxi;
                }

                //it high scores are on edges
                if (maxi == 0 || maxi == 3) result += 25000;
                if (boardMaxiX == 0 && boardMaxiY == 0 ||
                    boardMaxiX == 0 && boardMaxiY == 3 ||
                    boardMaxiX == 3 && boardMaxiY == 0 ||
                    boardMaxiX == 3 && boardMaxiY == 3) result += 250000;

                // penalty for low tiles around maxi
                if (i != 0)
                    if (board[i - 1, maxi] != 0)
                        result -= board[i, maxi] / board[i - 1, maxi] * 250;
                if (i != 3)
                    if (board[i + 1, maxi] != 0)
                        result -= board[i, maxi] / board[i + 1, maxi] * 250;

                if (maxi != 0)
                    if (board[i, maxi - 1] != 0)
                        result -= board[i, maxi] / board[i, maxi - 1] * 250;

                if (maxi != 3)
                    if (board[i, maxi + 1] != 0)
                        result -= board[i, maxi] / board[i, maxi + 1] * 250;


                // if high scores are close together and 1 power diff
                for (int j = 1; j < 4; j++)
                {
                    if ((board[i, j] == board[i, j - 1] * 2) || (board[i, j] == board[i, j - 1] / 2))
                    {
                        result += 2500;
                    }
                }

                // if they are arranged in a snake
                if (i % 2 == 1)
                {
                    if (board[i, 0] < board[i, 1] && board[i, 1] < board[i, 2] && board[i, 2] < board[i, 3]) result += 20000;
                }
                else
                {
                    if (board[i, 0] > board[i, 1] && board[i, 1] > board[i, 2] && board[i, 2] > board[i, 3]) result += 20000;
                }
            }

            return result + 100000;
        }

        static void Main(String[] args)
        {
            int[,] board = new int[4, 4];

            for (int i = 0; i < 4; i++)
            {
                String[] line_split = Console.ReadLine().Split(' ');
                for (int j = 0; j < 4; j++)
                    board[i, j] = Convert.ToInt32(line_split[j]);
            }

            Console.WriteLine(NextMove(board));
        }
        private static int[,] TransposeBoard(int[,] board)
        {
            int[,] result = new int[4, 4];

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    result[i, j] = board[j, i];

            return result;
        }

        public static bool BoardsAreTheSame(int[,] a, int[,] b)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (a[i, j] != b[i, j]) return false;

            return true;
        }

        #region execute move

        public static int[,] ExecuteMove(string move, int[,] board)
        {
            switch (move)
            {
                case "UP":
                    return ExecuteMoveUP(board);
                case "DOWN":
                    return ExecuteMoveDOWN(board);
                case "LEFT":
                    return ExecuteMoveLEFT(board);
                case "RIGHT":
                    return ExecuteMoveRIGHT(board);
                default:
                    return null;
            }
        }

        public static int[,] ExecuteMoveUP(int[,] board)
        {
            bool[,] boardMerged = new bool[4, 4];

            for (int i = 1; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var tmp = i;
                    while (tmp > 0)
                    {
                        if (board[tmp, j] == board[tmp - 1, j] && !boardMerged[tmp - 1, j] && board[tmp, j] != 0)
                        {
                            boardMerged[tmp - 1, j] = true;
                            board[tmp - 1, j] *= 2;
                            board[tmp, j] = 0;
                        }
                        else if (board[tmp - 1, j] == 0)
                        {
                            board[tmp - 1, j] = board[tmp, j];
                            board[tmp, j] = 0;
                        }
                        tmp--;
                    }
                }
            }
            return board;
        }

        public static int[,] ExecuteMoveDOWN(int[,] board)
        {
            bool[,] boardMerged = new bool[4, 4];

            for (int i = 3; i >= 0; i--)
            {
                for (int j = 0; j < 4; j++)
                {
                    var tmp = i;
                    while (tmp < 3)
                    {
                        if (board[tmp, j] == board[tmp + 1, j] && !boardMerged[tmp + 1, j] && board[tmp, j] != 0)
                        {
                            boardMerged[tmp + 1, j] = true;
                            board[tmp + 1, j] *= 2;
                            board[tmp, j] = 0;
                        }
                        else if (board[tmp + 1, j] == 0)
                        {
                            board[tmp + 1, j] = board[tmp, j];
                            board[tmp, j] = 0;
                        }
                        tmp++;
                    }
                }
            }
            return board;
        }
        public static int[,] ExecuteMoveLEFT(int[,] board)
        {
            bool[,] boardMerged = new bool[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    var tmp = j;
                    while (tmp > 0)
                    {
                        if (board[i, tmp] == board[i, tmp - 1] && !boardMerged[i, tmp - 1] && board[i, tmp] != 0)
                        {
                            boardMerged[i, tmp - 1] = true;
                            board[i, tmp - 1] *= 2;
                            board[i, tmp] = 0;
                        }
                        else if (board[i, tmp - 1] == 0)
                        {
                            board[i, tmp - 1] = board[i, tmp];
                            board[i, tmp] = 0;
                        }
                        tmp--;
                    }
                }
            }
            return board;
        }
        public static int[,] ExecuteMoveRIGHT(int[,] board)
        {
            bool[,] boardMerged = new bool[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 3; j >= 0; j--)
                {
                    var tmp = j;
                    while (tmp < 3)
                    {
                        if (board[i, tmp] == board[i, tmp + 1] && !boardMerged[i, tmp + 1] && board[i, tmp] != 0)
                        {
                            boardMerged[i, tmp + 1] = true;
                            board[i, tmp + 1] *= 2;
                            board[i, tmp] = 0;
                        }
                        else if (board[i, tmp + 1] == 0)
                        {
                            board[i, tmp + 1] = board[i, tmp];
                            board[i, tmp] = 0;
                        }
                        tmp++;
                    }
                }
            }
            return board;
        }
        #endregion
    }
}
