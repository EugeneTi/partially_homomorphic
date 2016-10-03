using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Mpir.NET;

namespace RSACryptography.Core
{
    static class MathExtended
    {
        public static mpz_t ModularLinearEquationSolver(mpz_t a, mpz_t b, mpz_t n)
        {
            mpz_t x;
            mpz_t y;
            mpz_t d;

            ExtendedEuclid(a, n, out d, out x, out y);

            if (b % d == 0)
            {
                x = (x * (b / d)) % n;

                if (x < 0)
                {
                    return (x + n) % n;
                }
                return x;
            }
            return -1;
        }

        static void ExtendedEuclid(mpz_t a, mpz_t b, out mpz_t d, out mpz_t lastx, out mpz_t lasty)
        {
            mpz_t x = 0;
            mpz_t y = 1;

            lastx = 1;
            lasty = 0;

            while (b != 0)
            {
                mpz_t quotient = a / b;
                mpz_t temp = b;

                b = a % b;
                a = temp;

                temp = x;
                x = lastx - quotient * x;
                lastx = temp;

                temp = y;
                y = lasty - quotient * y;
                lasty = temp;
            }

            d = a;
        }
    }
}
