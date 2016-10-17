using System.Numerics;
using Mpir.NET;

namespace Cryptography.RSA
{
    public class RSAManager
    {
        private static RSAManager _default;
        public static RSAManager Default
        {
            get
            {
                if (_default != null)
                    return _default;

                Generator.Initialize(2);
                var p = new PrimeNumber();
                mpz_t numMin = ((mpz_t)2).Power((BitLength / 2) - 1);
                p.SetNumber(Generator.Random(numMin, numMin));
                p.RabinMiller();
                _default = new RSAManager {PublicPrime = p.GetPrimeNumber()*p.GetPrimeNumber()};
                return _default;
            }
        }

        public mpz_t PublicPrime { get; set; }

        public static int BitLength = 1024;

        public RSAKey GenerateKey()
        {
            mpz_t numMin = ((mpz_t)2).Power((BitLength / 2) - 1);// BigInteger.Pow(2, (BitLength / 2) - 1);
            mpz_t numMax = ((mpz_t)2).Power((BitLength / 2)); //BigInteger.Pow(2, (BitLength / 2));

            var p = new PrimeNumber();
            var q = new PrimeNumber();
            var e = new PrimeNumber(); //open key
            var d = new mpz_t(); //secret key
            var n = new mpz_t(); //module
            
            p.SetNumber(Generator.Random(numMin, numMin));
            q.SetNumber(Generator.Random(numMin, numMax));

            p.RabinMiller();
            q.RabinMiller();

            n = p.GetPrimeNumber() * q.GetPrimeNumber();
            mpz_t eulersPhiFunction = (p.GetPrimeNumber() - 1) * (q.GetPrimeNumber() - 1);

            while (true)
            {
                e.SetNumber(Generator.Random(2, new mpz_t(eulersPhiFunction - 1)));
                e.RabinMiller();

                if (e.GetFoundPrime() && (mpz_t.Gcd(e.GetPrimeNumber(), eulersPhiFunction) == 1))
                {
                    break;
                }
            }

            d = MathExtended.ModularLinearEquationSolver(new mpz_t(e.GetPrimeNumber()), 1, new mpz_t(eulersPhiFunction));

            var key = new RSAKey();
            key.PrivateKey = d;
            key.OpenKey = e.GetPrimeNumber();
            key.P = n;
            key.PublicPrime = PublicPrime;

            return key;
        }
    }
}
