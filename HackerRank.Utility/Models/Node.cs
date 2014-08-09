using System;
using System.Collections.Generic;

namespace HackerRank.Utility.Models
{
    public class Node<T> where T : class
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
}
