using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Utils
{
    public class LcmGcd
    {
        public static long gcd(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b > 0)
            {
                long rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static long lcm(long a, long b)
        {
            return (a / gcd(a, b)) * b;
        }
    }
}
