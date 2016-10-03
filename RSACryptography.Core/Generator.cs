using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Mpir.NET;

namespace RSACryptography.Core
{
    static class Generator
    {
        private static int _type;

        private static mpz_t _m = ((mpz_t) 2).Power(32);
        private static mpz_t _a = 69069;
        private static mpz_t _b = 0;
        private static mpz_t _rn = 1;

        private static Random _rnd = new Random();

        public static void Initialize(int t)
        {
            _type = t;

            if (_type == 0)
            {
                _m = ((mpz_t)2).Power(32);
                _a = 69069;
                _b = 0;
                _rn = 1;
            }
            else if (_type == 1)
            {
                _rnd = new Random();
            }
            else
            {
                _rnd = new Random();
            }
        }

        public static void SetLcg(mpz_t mIn, mpz_t aIn, mpz_t bIn, mpz_t rnIn)
        {
            _type = 0;

            _m = mIn;
            _a = aIn;
            _b = bIn;
            _rn = rnIn;
        }

        public static mpz_t Random(mpz_t a, mpz_t b)
        {
            mpz_t retValue;

            if (_type == 0)
            {
                retValue = a + Lcg() % (b - a + 1);
            }
            else if (_type == 1)
            {
                mpz_t count = b - a;

                BigInteger digits = 0;

                while ((count / 10) > 0)
                {
                    count = count / 10;

                    digits++;
                }

                string entropy = Entropy();

                string retVal = "";

                while (retVal.Length < digits)
                {
                    retVal = retVal + entropy[_rnd.Next(0, entropy.Length)];
                }

                // We get the number, but we might be too high, so we do a mod
                retValue = new mpz_t(BigInteger.Parse(retVal));

                retValue = a + (retValue % b);
            }
            else
            {
                mpz_t count = b - a;

                mpz_t digits = 0;

                while ((count / 10) > 0)
                {
                    count = count / 10;

                    digits++;
                }

                string retVal = _rnd.Next(1000000000, 2100000000).ToString(CultureInfo.InvariantCulture);

                while (retVal.Length < digits)
                {
                    retVal = retVal + _rnd.Next(1000).ToString(CultureInfo.InvariantCulture);
                }

                // We get the number, but we might be too high, so we do a mod
                retValue = new mpz_t(BigInteger.Parse(retVal));

                retValue = a + (retValue % b);
            }

            return retValue;
        }

        private static string Entropy()
        {
            string entropy = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

            PerformanceCounter cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };

            entropy = entropy + cpuCounter.NextValue();

            return entropy;
        }

        private static mpz_t Lcg()
        {
            _rn = (_a * _rn + _b) % _m;
            return _rn;
        }
    }
}
