﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PacmanDFS
{
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

    class ExploredPosition
    {
        public Position<int> Position { get; set; }
        public Node<Position<int>> Parent { get; set; }

        public ExploredPosition(Position<int> position, Node<Position<int>> parent)
        {
            this.Position = position;
            this.Parent = parent;
        }
    }

    class State
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Position<int> PacmanPosition { get; set; }
        public Position<int> FoodPosition { get; set; }
        public String[] Grid { get; set; }

        public State DeepCopy()
        {
            return new State
            {
                Rows = this.Rows,
                Columns = this.Columns,
                PacmanPosition = new Position<int>(this.PacmanPosition.I, this.PacmanPosition.J),
                FoodPosition = new Position<int>(this.FoodPosition.I, this.FoodPosition.J),
                Grid = this.Grid
            };
        }
    }

    class Solution
    {
        private static Tree<Position<int>> Tree { get; set; }
        private static Stack<ExploredPosition> Explored { get; set; }
        private static List<Position<int>> Expanded { get; set; }
        private static Node<Position<int>> FoodNode { get; set; }
        private static bool ResultFound { get; set; }

        private static void dfs(State state)
        {
            // INIT
            Tree = new Tree<Position<int>>(state.PacmanPosition);
            Explored = new Stack<ExploredPosition>();
            Expanded = new List<Position<int>>{ state.PacmanPosition };

            EvaluateOptions(state, Tree.Root);

            var reversePath = new List<Position<int>> { FoodNode.Value };
            var parent = FoodNode.Parent;
            while (parent != null)
            {
                reversePath.Add(parent.Value);
                parent = parent.Parent;
            }

            if (ResultFound)
            {
                Console.WriteLine(Expanded.Count);
                foreach (var position in Expanded)
                    Console.WriteLine(position.I + " " + position.J);
                Console.WriteLine(reversePath.Count-1);
                for (var i = reversePath.Count; i > 0; i--)
                    Console.WriteLine(reversePath[i-1].I + " " + reversePath[i-1].J);
            }
        }

        private static void EvaluateOptions(State state, Node<Position<int>> parentNode)
        {
            ExploreNeighbours(state, parentNode);

            while (Explored.Any())
            {
                if (ResultFound) break;
                var next = Explored.Peek();
                var nextState = NewState(next.Position, state);
                if (!Expanded.Any(x => x.I == nextState.PacmanPosition.I && x.J == nextState.PacmanPosition.J))
                    NextMove(nextState, next.Parent);
            }
        }
        private static State NewState(Position<int> position, State state)
        {
            var newState = state.DeepCopy();

            newState.PacmanPosition = position;

            return newState;
        }

        private static State NewState(string direction, State state)
        {
            var newState = state.DeepCopy();

            switch (direction)
            {
                case "DOWN":
                    newState.PacmanPosition.I++;
                    break;
                case "RIGHT":
                    newState.PacmanPosition.J++;
                    break;
                case "LEFT":
                    newState.PacmanPosition.J--;
                    break;
                case "UP":
                    newState.PacmanPosition.I--;
                    break;
            }

            return newState;
        }

        private static void ExploreNeighbours(State state, Node<Position<int>> parentNode)
        {
            Explore(NewState("UP", state), parentNode);
            Explore(NewState("LEFT", state), parentNode);
            Explore(NewState("RIGHT", state), parentNode);
            Explore(NewState("DOWN", state), parentNode);
        }

        static void Explore(State state, Node<Position<int>> parentNode)
        {
            if (ResultFound) return;

            if (state.Grid[state.PacmanPosition.I][state.PacmanPosition.J] != '-' &&
                state.Grid[state.PacmanPosition.I][state.PacmanPosition.J] != '.') return;

            if (Explored.Any(x => x.Position.I == state.PacmanPosition.I && x.Position.J == state.PacmanPosition.J)) return;

            if (!Expanded.Any(x => x.I == state.PacmanPosition.I && x.J == state.PacmanPosition.J))
                Explored.Push(new ExploredPosition(state.PacmanPosition, parentNode));
        }

        private static void Check(State state, Node<Position<int>> parentNode)
        {
        }

        private static void NextMove(State state, Node<Position<int>> parentNode)
        {
            switch (state.Grid[state.PacmanPosition.I][state.PacmanPosition.J])
            {
                case '-':
                    parentNode.AddChild(state.PacmanPosition);
                    Expanded.Add(state.PacmanPosition);
                    Explored.Pop();
                    ExploreNeighbours(state, parentNode.Children[parentNode.Children.Count - 1]);
                    break;
                case '.':
                    FoodNode = parentNode.AddChild(state.PacmanPosition);
                    Expanded.Add(state.PacmanPosition);
                    Explored.Pop();
                    ResultFound = true;
                    break;
            }
        }

        private static void Main(String[] args)
        {
            int r, c;
            int pacman_r, pacman_c;
            int food_r, food_c;

            String pacman = Console.ReadLine();
            String food = Console.ReadLine();
            String pos = Console.ReadLine();

            String[] pos_split = pos.Split(' ');
            String[] pacman_split = pacman.Split(' ');
            String[] food_split = food.Split(' ');

            r = Convert.ToInt32(pos_split[0]);
            c = Convert.ToInt32(pos_split[1]);

            pacman_r = Convert.ToInt32(pacman_split[0]);
            pacman_c = Convert.ToInt32(pacman_split[1]);

            food_r = Convert.ToInt32(food_split[0]);
            food_c = Convert.ToInt32(food_split[1]);

            String[] grid = new String[r];

            for (int i = 0; i < r; i++)
            {
                grid[i] = Console.ReadLine();
            }

            var state = new State
            {
                Rows = r,
                Columns = c,
                PacmanPosition = new Position<int>(pacman_r, pacman_c),
                FoodPosition = new Position<int>(food_r, food_c),
                Grid = grid
            };

            dfs(state);
        }
    }
}
