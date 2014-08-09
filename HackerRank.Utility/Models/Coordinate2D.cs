
namespace HackerRank.Utility.Models
{
    public class Coordinate2D<T> where T : struct 
    {
        public T X { get; set; }
        public T Y { get; set; }

        public Coordinate2D(T x, T y)
        {
            X = x;
            Y = y;
        }
    }
}
