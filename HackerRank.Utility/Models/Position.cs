
namespace HackerRank.Utility.Models
{
    public class Position<T> where T : struct
    {
        public T I { get; set; }
        public T J { get; set; }

        public Position(T i, T j)
        {
            I = i;
            J = j;
        }
    }
}
