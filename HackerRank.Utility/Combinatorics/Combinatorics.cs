using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank.Utility
{
    public static class Combinatorics
    {
        /// <summary>
        /// Returns the number of combinations of n things taken k at a time without repetition
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int Combinations(int n, int k)
        {
            return Algebra.Factorial(n)/(Algebra.Factorial(k)*Algebra.Factorial(n - k));
        }
    }
}
