using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NPuzzle
{
    public class PriorityQueue<TPriority, TValue>
    {
        private List<KeyValuePair<TPriority, TValue>> _baseHeap;
        private IComparer<TPriority> _comparer;

        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException();

            _baseHeap = new List<KeyValuePair<TPriority, TValue>>();
            _comparer = comparer;
        }

        public void Enqueue(TPriority priority, TValue value)
        {
            Insert(priority, value);
        }

        private void Insert(TPriority priority, TValue value)
        {
            KeyValuePair<TPriority, TValue> val =
                new KeyValuePair<TPriority, TValue>(priority, value);
            _baseHeap.Add(val);

            HeapifyFromEndToBeginning(_baseHeap.Count - 1);
        }

        private int HeapifyFromEndToBeginning(int pos)
        {
            if (pos >= _baseHeap.Count) return -1;

            while (pos > 0)
            {
                int parentPos = (pos - 1) / 2;
                if (_comparer.Compare(_baseHeap[parentPos].Key, _baseHeap[pos].Key) > 0)
                {
                    ExchangeElements(parentPos, pos);
                    pos = parentPos;
                }
                else break;
            }
            return pos;
        }

        private void ExchangeElements(int pos1, int pos2)
        {
            KeyValuePair<TPriority, TValue> val = _baseHeap[pos1];
            _baseHeap[pos1] = _baseHeap[pos2];
            _baseHeap[pos2] = val;
        }

        public TValue DequeueValue()
        {
            return Dequeue().Value;
        }

        public KeyValuePair<TPriority, TValue> Dequeue()
        {
            if (!IsEmpty)
            {
                KeyValuePair<TPriority, TValue> result = _baseHeap[0];
                DeleteRoot();
                return result;
            }
            else
                throw new InvalidOperationException("Priority queue is empty");
        }

        private void DeleteRoot()
        {
            if (_baseHeap.Count <= 1)
            {
                _baseHeap.Clear();
                return;
            }

            _baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
            _baseHeap.RemoveAt(_baseHeap.Count - 1);

            HeapifyFromBeginningToEnd(0);
        }

        private void HeapifyFromBeginningToEnd(int pos)
        {
            if (pos >= _baseHeap.Count) return;

            while (true)
            {
                int smallest = pos;
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                if (left < _baseHeap.Count &&
                    _comparer.Compare(_baseHeap[smallest].Key, _baseHeap[left].Key) > 0)
                    smallest = left;
                if (right < _baseHeap.Count &&
                    _comparer.Compare(_baseHeap[smallest].Key, _baseHeap[right].Key) > 0)
                    smallest = right;

                if (smallest != pos)
                {
                    ExchangeElements(smallest, pos);
                    pos = smallest;
                }
                else break;
            }
        }

        public KeyValuePair<TPriority, TValue> Peek()
        {
            if (!IsEmpty)
                return _baseHeap[0];
            else
                throw new InvalidOperationException("Priority queue is empty");
        }

        public TValue PeekValue()
        {
            return Peek().Value;
        }

        public bool IsEmpty
        {
            get { return _baseHeap.Count == 0; }
        }
    }

    class Position<T> where T : struct
    {
        public T I { get; set; }
        public T J { get; set; }

        public Position(T i, T j)
        {
            I = i;
            J = j;
        }
    }

    class Tree<T> where T : class
    {
        public Node<T> Root { get; set; }

        public Tree(T item)
        {
            this.Root = new Node<T>(item);
        }
    }

    class Node<T> where T : class
    {
        public Node<T> Parent { get; set; }
        public List<Node<T>> Children { get; set; }

        public int Depth
        {
            get
            {
                var parent = this.Parent;
                var i = 0;
                while (parent != null)
                {
                    i++;
                    parent = parent.Parent;
                }
                return i;
            }
        }

        public Node<T> Child(int index)
        {
            try
            {
                return Children[index];
            }
            catch (IndexOutOfRangeException ex)
            {
                return null;
            }
        }

        public T Value;

        public Node(T value)
        {
            this.Value = value;
            this.Children = new List<Node<T>>();
        }

        public Node<T> AddChild(T item)
        {
            Node<T> child = new Node<T>(item);
            this.Children.Add(child);
            child.Parent = this;
            return child;
        }
    }

    class State
    {
        public int Size { get; set; }
        public int[,] Board { get; set; }
        public string BoardKey { get; set; }

        public State DeepCopy()
        {
            var newBoard = new int[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    newBoard[i, j] = Board[i, j];
                }
            }

            return new State
            {
                Size = this.Size,
                Board = newBoard,
                BoardKey = this.BoardKey
            };
        }
    }

    class ExploredPosition
    {
        public string Move { get; set; }
        public State State { get; set; }
        public Node<string> Parent { get; set; }

        public ExploredPosition(string move, State state, Node<string> parent)
        {
            this.Move = move;
            this.State = state;
            this.Parent = parent;
        }
    }

    class Solution
    {
        private static ConcurrentDictionary<string, int> StateDistanceDictionary { get; set; }
        private static PriorityQueue<int, ExploredPosition> Explored { get; set; }
        private static List<string> Expanded { get; set; }
        private static Tree<string> Tree { get; set; }
        private static Node<string> ResultNode { get; set; }
        private static string ResultBoardKey { get; set; }
        private static bool ResultFound { get; set; }

        private static void Astar(State state)
        {
            // INIT
            Tree = new Tree<string>("START");
            StateDistanceDictionary = new ConcurrentDictionary<string, int>();
            Explored = new PriorityQueue<int, ExploredPosition>();
            Expanded = new List<string> { state.BoardKey };

            EvaluateOptions(state, Tree.Root);

            if (ResultFound)
            {
                var reversePath = new List<string> { ResultNode.Value };
                var parent = ResultNode.Parent;
                while (parent.Value != "START")
                {
                    reversePath.Add(parent.Value);
                    parent = parent.Parent;
                }

                Console.WriteLine(reversePath.Count);
                for (var i = reversePath.Count; i > 0; i--)
                    Console.WriteLine(reversePath[i - 1]);
            }
        }

        private static void EvaluateOptions(State state, Node<string> parentNode)
        {
            if (state.BoardKey == ResultBoardKey)
            {
                ResultNode = parentNode;
                ResultFound = true;
                return;
            }

            ExploreNeighbours(state, parentNode);

            while (!Explored.IsEmpty)
            {
                if (ResultFound) break;
                var next = Explored.PeekValue();
                if (Expanded.All(x => x != next.State.BoardKey))
                    NextMove(next.State, next.Parent, next.Move);
                else
                    Explored.Dequeue();
            }
        }

        private static void ExploreNeighbours(State state, Node<string> parentNode)
        {
            Explore("UP", state, parentNode);
            Explore("LEFT", state, parentNode);
            Explore("RIGHT", state, parentNode);
            Explore("DOWN", state, parentNode);
        }

        private static void Explore(string direction, State state, Node<string> parentNode)
        {
            if (ResultFound) return;

            var nextState = ExecuteMove(direction, state);

            if (nextState != null && Expanded.All(x => x != nextState.BoardKey))
                Explored.Enqueue(GetDistance(nextState), new ExploredPosition(direction, nextState, parentNode));
        }

        private static void NextMove(State state, Node<string> parentNode, string move)
        {
            if (state.BoardKey == ResultBoardKey)
            {
                ResultNode = parentNode.AddChild(move);
                Expanded.Add(state.BoardKey);
                Explored.Dequeue();
                ResultFound = true;
                return;
            }

            parentNode.AddChild(move);
            Expanded.Add(state.BoardKey);
            Explored.Dequeue();
            ExploreNeighbours(state, parentNode.Children[parentNode.Children.Count - 1]);
        }

        private static State ExecuteMove(string move, State state)
        {
            var newState = state.DeepCopy();
            var position0 = new Position<int>(newState.BoardKey.IndexOf('0') / newState.Size, newState.BoardKey.IndexOf('0') % newState.Size);

            switch (move)
            {
                case "UP":
                    if (position0.I == 0) return null;
                    SwapPositions(newState.Board, position0, new Position<int>(position0.I - 1, position0.J));
                    break;
                case "DOWN":
                    if (position0.I == newState.Size - 1) return null;
                    SwapPositions(newState.Board, position0, new Position<int>(position0.I + 1, position0.J));
                    break;
                case "LEFT":
                    if (position0.J == 0) return null;
                    SwapPositions(newState.Board, position0, new Position<int>(position0.I, position0.J - 1));
                    break;
                case "RIGHT":
                    if (position0.J == newState.Size - 1) return null;
                    SwapPositions(newState.Board, position0, new Position<int>(position0.I, position0.J + 1));
                    break;
            }

            newState.BoardKey = "";
            for (var i = 0; i < newState.Size * newState.Size; i++)
                newState.BoardKey += newState.Board[i / newState.Size, i % newState.Size];

            return newState;
        }

        private static void SwapPositions(int[,] board, Position<int> origPosition, Position<int> newPosition)
        {
            var tmp = board[newPosition.I, newPosition.J];
            board[newPosition.I, newPosition.J] = board[origPosition.I, origPosition.J];
            board[origPosition.I, origPosition.J] = tmp;
        }

        private static int GetDistance(State state)
        {
            int distance;

            if (StateDistanceDictionary.TryGetValue(state.BoardKey, out distance))
            {
                return distance;
            }

            distance = 0;
            var misplaced = 0;
            for (int i = 0; i < state.Size; i++)
            {
                for (int j = 0; j < state.Size; j++)
                {
                    if (state.Board[i, j] != i * state.Size + j) misplaced++;
                    distance += Math.Abs(i - state.Board[i, j] / state.Size) + Math.Abs(j - state.Board[i, j] % state.Size);
                }
            }


            if (misplaced > distance)
            {
                StateDistanceDictionary.GetOrAdd(state.BoardKey, misplaced);
                return misplaced;
            }

            StateDistanceDictionary.GetOrAdd(state.BoardKey, distance);
            return distance;
        }

        private static void Main(String[] args)
        {

            String firstLine = Console.ReadLine();

            var n = Convert.ToInt32(firstLine);
            var board = new int[n, n];
            var boardKey = "";

            for (var i = 0; i < n * n; i++)
            {
                var line = Console.ReadLine();
                board[i / n, i % n] = Convert.ToInt32(line);
                boardKey += line;
            }

            var state = new State
            {
                Size = n,
                Board = board,
                BoardKey = boardKey
            };

            ResultBoardKey = "";
            for (int i = 0; i < state.Size * state.Size; i++)
            {
                ResultBoardKey += i;
            }

            Astar(state);
        }
    }
}
