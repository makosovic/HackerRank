using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank.Utility
{
    public static class Algebra
    {
        /// <summary>
        /// Calculates n factorial
        /// </summary>
        /// <param name="n"></param>
        /// <returns>int</returns>
        public static int Factorial(int n)
        {
            if (n <= 1)
                return 1;
            return n * Factorial(n - 1);
        }
    }
}
