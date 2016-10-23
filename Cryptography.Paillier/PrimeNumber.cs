using System;
using System.Numerics;
using Mpir.NET;

namespace Cryptography.Paillier
{
    class PrimeNumber
    {
        private mpz_t _iterationTested = 0;
        private mpz_t _numberTested = 0;
        private mpz_t _number = 2;

        private mpz_t _s = 10;

        private bool _found;

        private bool _running;

        public mpz_t GetPrimeNumber()
        {
            return _number;
        }

        public mpz_t GetTestedCount()
        {
            return _numberTested;
        }

        public mpz_t GetTestedIterations()
        {
            return _iterationTested;
        }

        public bool GetFoundPrime()
        {
            return _found;
        }

        public void StopEngine()
        {
            _running = false;
        }

        public void TestRabinMiller()
        {
            _found = false;
            _running = true;

            if (_number == 2)
            {
                _found = true;
            }

            if (_number % 2 != 0)
            {
                _found = RabinMillerTest(_number, _s);
            }

            _running = false;
        }

        public void TestNaive()
        {
            _found = false;
            _running = true;

            if (_number == 2)
            {
                _found = true;
            }

            if (_number % 2 != 0)
            {
                _found = NaiveTest(_number);
            }

            _running = false;
        }

        private bool NaiveTest(mpz_t r)
        {
            if (r < 3)
            {
                return true;
            }

            _iterationTested = 0;

            mpz_t g = Sqrt(r);

            mpz_t j = 3;

            while (j <= g)
            {
                _iterationTested++;

                if (_number % j == 0)
                {
                    break;
                }

                j = j + 2;
            }

            if (j > g)
            {
                return true;
            }
            return false;
        }

        private bool RabinMillerTest(mpz_t r, mpz_t s)
        {
            _iterationTested = 0;

            //
            // Find D and K so equality is correct: d*2^k = r - 1
            //

            mpz_t d = r - 1;
            mpz_t k = 0;

            while (d % 2 == 0)
            {
                d = d / 2;
                k = k + 1;
            }

            for (mpz_t j = 1; j <= s; j++)
            {
                _iterationTested++;

                mpz_t a = Generator.Random(2, (r - 1));
                mpz_t x = a.PowerMod(d, r);// BigInteger.ModPow(a, d, r);

                if (x != 1)
                {
                    for (mpz_t i = 0; i < (k - 1); i++)
                    {
                        if (x == _number - 1)
                        {
                            break;
                        }

                        x = x.PowerMod(2, _number);
                    }

                    if (x != _number - 1)
                    {
                        return false;
                    }
                }

                if (_running == false)
                {
                    return false;
                }
            }

            return true;
        }

        private mpz_t Sqrt(mpz_t n)
        {
            if (n == 0)
            {
                return 0;
            }

            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n.ToBigInteger(), 2)));
                mpz_t root = mpz_t.One << (bitLength / 2);

                while (!IsSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private bool IsSqrt(mpz_t n, mpz_t root)
        {
            mpz_t lowerBound = root * root;
            mpz_t upperBound = (root + 1) * (root + 1);

            return ((n >= lowerBound) && (n < upperBound));
        }


        public void SetNumber(mpz_t num)
        {
            _number = num;
        }

        public void SetRabinMiller(mpz_t sNew)
        {
            if (sNew < 2)
            {
                sNew = 2;
            }

            _s = sNew;
        }

        public void RabinMiller()
        {
            _running = true;
            _found = false;

            // No negative primes
            if (_number < 1)
            {
                _number = 1;
            }

            // Two is prime
            if (_number <= 2)
            {
                return;
            }

            // Other even numbers arent primes
            if (_number % 2 == 0)
            {
                _number = _number + 1;
            }

            // First five is prime
            if (_number == 5)
            {
                _running = false;
                _found = true;

                return;
            }

            _numberTested = 0;

            while (_running)
            {
                if (RabinMillerTest(_number, _s))
                {
                    _found = true;
                    _running = false;

                    return;
                }

                // Skip number 5
                if (_number % 10 == 3)
                {
                    _number = _number + 4;
                }
                else
                {
                    _number = _number + 2;
                }

                _numberTested = _numberTested + 1;
            }
        }

        public void Naive()
        {
            _running = true;
            _found = false;

            _numberTested = 0;

            // No negative primes
            if (_number < 1)
            {
                _number = 1;
            }

            // Two is prime
            if (_number <= 2)
            {
                return;
            }

            // Other even numbers arent primes
            if (_number % 2 == 0)
            {
                _number = _number + 1;
            }

            // First five is prime
            if (_number == 5)
            {
                _running = false;
                _found = true;

                return;
            }

            while (_running)
            {
                if (NaiveTest(_number))
                {
                    _found = true;
                    _running = false;

                    return;
                }

                // Skip number 5
                if (_number % 10 == 3)
                {
                    _number = _number + 4;
                }
                else
                {
                    _number = _number + 2;
                }

                _numberTested = _numberTested + 1;
            }
        }
    }
}
