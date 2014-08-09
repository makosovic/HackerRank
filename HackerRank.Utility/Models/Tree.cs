
namespace HackerRank.Utility.Models
{
    public class Tree<T> where T : class
    {
        public Node<T> Root { get; set; }

        public Tree(T item)
        {
            this.Root = new Node<T>(item);
        }
    }
}
