
namespace HackerRank.Utility.Models
{
    public class Vector
    {
        public double this[int n]
        {
            get { return _vector[n]; }
            set { _vector[n] = value; }
        }

        public int Length { get { return _vector.Length; } }

        private double[] _vector;

        public Vector(int n)
        {
            _vector = new double[n];
        }

        public Vector(double[] vector)
        {
            _vector = vector;
        }

        public double[] Value()
        {
            return _vector;
        }
    }
}
