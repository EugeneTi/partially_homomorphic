using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Mpir.NET;

namespace Cryptography.Paillier
{
    public class PaillierManager
    {
        private static PaillierManager _default;
        public static PaillierManager Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new PaillierManager();
                }

                return _default;
            }
        }

        public mpz_t PublicPrime { get; set; }

        public static int BitLength = 2048;
        
        public mpz_t EncryptNumber(int number, PaillierKey key)
        {
            mpz_t r = RandomZStarN(key.N);
            var nsq = key.N*key.N;
            // c = g^m * r^n mod n^2
            return (key.G.PowerMod(number, nsq).Multiply(r.PowerMod(key.N, nsq))).Mod(nsq);
        }

        public mpz_t Multiply(mpz_t a, mpz_t b, PaillierKey key)
        {
            var nsquare = key.N * key.N;
            var res = (a*b)%nsquare;
            return res;
        }

        public mpz_t DecryptNumber(mpz_t encryptedNumber, PaillierKey key)
        {
            var nsquare = key.N * key.N;
            var m = encryptedNumber.PowerMod(key.Lambda, nsquare).Subtract(1).Divide(key.N).Multiply(key.Mu).Mod(key.N);
            return m;
        }

        public PaillierKey GenerateKey()
        {
            var p = new PrimeNumber();
            var q = new PrimeNumber();
            GeneratePrimes(out p, out q);

            // lambda = lcm(p-1, q-1) = (p-1)*(q-1)/gcd(p-1, q-1)
            var lambda = ((p.GetPrimeNumber() - 1)*(q.GetPrimeNumber() - 1))/
                         mpz_t.Gcd(p.GetPrimeNumber() - 1, q.GetPrimeNumber() - 1);
            var n = p.GetPrimeNumber() * q.GetPrimeNumber();    // n = p*q
            var nsquare = n * n;                                // nsquare = n*n

            mpz_t g; 
            do
            {
                // generate g, a random integer in Z*_{n^2}
                g = RandomZStarNSquare(nsquare);
            }
            // verify g, the following must hold: gcd(L(g^lambda mod n^2), n) = 1, where L(u) = (u-1)/n
            while (mpz_t.Gcd(g.PowerMod(lambda, nsquare).Subtract(1).Divide(n), n) != 1);

            // mu = (L(g^lambda mod n^2))^{-1} mod n, where L(u) = (u-1)/n
            var mu = g.PowerMod(lambda, nsquare).Subtract(1).Divide(n).InvertMod(n);
            

            var key = new PaillierKey
            {
                N = n,
                G = g,
                Lambda = lambda,
                Mu = mu
            };

            return key;
        }

        /// <summary>
        /// Return a random integer in Z*_{n^2}
        /// </summary>
        public mpz_t RandomZStarNSquare(mpz_t nsquare)
        {
            mpz_t r;

            do
            {
                r = GetRandomNumber();
            }
            while (r.CompareTo(nsquare) >= 0 || mpz_t.Gcd(r, nsquare) != 1);

            return r;
        }

        public mpz_t RandomZStarN(mpz_t n)
        {
            mpz_t r;

            do
            {
                r = GetRandomNumber();
            }
            while (r.CompareTo(n) >= 0 || mpz_t.Gcd(r , n) != 1);

            return r;
        }

        public mpz_t GetRandomNumber()
        {
            mpz_t numMin = ((mpz_t)2).Power((BitLength / 2) - 1);
            mpz_t numMax = ((mpz_t)2).Power((BitLength / 2));

            return Generator.Random(numMin, numMax);
        }

        private void GeneratePrimes(out PrimeNumber p, out PrimeNumber q)
        {
            p = new PrimeNumber();
            q = new PrimeNumber();

            mpz_t numMin = ((mpz_t)2).Power((BitLength / 2) - 1);
            mpz_t numMax = ((mpz_t)2).Power((BitLength / 2));

            do
            {
                p.SetNumber(Generator.Random(numMin, numMin));
                q.SetNumber(Generator.Random(numMin, numMax));

                p.RabinMiller();
                q.RabinMiller();

            } while (mpz_t.Gcd(p.GetPrimeNumber()*q.GetPrimeNumber(), (p.GetPrimeNumber() - 1)*(q.GetPrimeNumber() - 1)) != 1);
        }
    }

//    public class Paillier
//    {
//        private int CERTAINTY = 64;       // certainty with which primes are generated: 1-2^(-CERTAINTY)
//        private int modLength;                  // length in bits of the modulus n
//        private BigInteger p;                   // a random prime
//        private BigInteger q;                   // a random prime (distinct from p)
//        private BigInteger lambda;              // lambda = lcm(p-1, q-1) = (p-1)*(q-1)/gcd(p-1, q-1)
//        private BigInteger n;                   // n = p*q
//        private BigInteger nsquare;             // nsquare = n*n
//        private BigInteger g;                   // a random integer in Z*_{n^2}
//        private BigInteger mu;                  // mu = (L(g^lambda mod n^2))^{-1} mod n, where L(u) = (u-1)/n

//        public Paillier(int modLengthIn)
//        {
//        if (modLengthIn < 8)
//            throw new Exception("Paillier(int modLength): modLength must be >= 8");

//        modLength = modLengthIn;


//        generateKeys();
//    }

//    public BigInteger getP()
//    {
//        return p;
//    }

//    public BigInteger getQ()
//    {
//        return q;
//    }

//    public BigInteger getLambda()
//    {
//        return lambda;
//    }

//    public int getModLength()
//    {
//        return modLength;
//    }

//    public BigInteger getN()
//    {
//        return n;
//    }

//    public BigInteger getNsquare()
//    {
//        return nsquare;
//    }

//    public BigInteger getG()
//    {
//        return g;
//    }

//    public BigInteger getMu()
//    {
//        return mu;
//    }

//    public void generateKeys()
//    {
//        p = new BigInteger(modLength / 2, CERTAINTY, new Random());     // a random prime

//        do
//        {
//            q = new BigInteger(modLength / 2, CERTAINTY, new Random()); // a random prime (distinct from p)
//        }
//        while (q.compareTo(p) == 0);

//        // lambda = lcm(p-1, q-1) = (p-1)*(q-1)/gcd(p-1, q-1)
//        lambda = (p.Subtract(BigInteger.One).Multiply(q.subtract(BigInteger.ONE))).divide(
//                p.subtract(BigInteger.ONE).gcd(q.subtract(BigInteger.ONE)));

//        n = p.multiply(q);              // n = p*q
//        nsquare = n.multiply(n);        // nsquare = n*n

//        do
//        {
//            // generate g, a random integer in Z*_{n^2}
//            g = randomZStarNSquare();
//        }
//        // verify g, the following must hold: gcd(L(g^lambda mod n^2), n) = 1, where L(u) = (u-1)/n
//        while (g.modPow(lambda, nsquare).subtract(BigInteger.ONE).divide(n).gcd(n).intValue() != 1);

//        // mu = (L(g^lambda mod n^2))^{-1} mod n, where L(u) = (u-1)/n
//        mu = g.modPow(lambda, nsquare).subtract(BigInteger.ONE).divide(n).modInverse(n);
//    }

//    public BigInteger encrypt(BigInteger m)
//    {
        
//        // generate r, a random integer in Z*_n
//        BigInteger r = randomZStarN();
        
//        // c = g^m * r^n mod n^2
//        return (g.modPow(m, nsquare).multiply(r.modPow(n, nsquare))).mod(nsquare);
//    }

//    public BigInteger encrypt(BigInteger m, BigInteger r) 
//    {
//        // if m is not in Z_n
//        if (m.CompareTo(BigInteger.Zero) < 0 || m.CompareTo(n) >= 0)
//        {
//            throw new Exception("Paillier.encrypt(BigInteger m, BigInteger r): plaintext m is not in Z_n");
//        }
        
//        // if r is not in Z*_n
//        if (r.CompareTo(BigInteger.ZERO) < 0 || r.compareTo(n) >= 0 || r.gcd(n).intValue() != 1)
//        {
//            throw new Exception("Paillier.encrypt(BigInteger m, BigInteger r): random integer r is not in Z*_n");
//        }
        
//        // c = g^m * r^n mod n^2
//        return (g.ModPow(m, nsquare).multiply(r.modPow(n, nsquare))).mod(nsquare);
//    }

//    public BigInteger decrypt(BigInteger c) 
//    {
//        // if c is not in Z*_{n^2}
//        if (c.compareTo(BigInteger.ZERO) < 0 || c.compareTo(nsquare) >= 0 || c.gcd(nsquare).intValue() != 1)
//        {
//            throw new Exception("Paillier.decrypt(BigInteger c): ciphertext c is not in Z*_{n^2}");
//        }
        
//        // m = L(c^lambda mod n^2) * mu mod n, where L(u) = (u-1)/n
//        return c.modPow(lambda, nsquare).subtract(BigInteger.ONE).divide(n).multiply(mu).mod(n);
//    }

    

//    // return a random integer in Z_n
//    public BigInteger RandomZN()
//    {
//        BigInteger r;

//        do
//        {
//            r = new BigInteger(modLength, new Random());
//        }
//        while (r.CompareTo(BigInteger.Zero) <= 0 || r.CompareTo(n) >= 0);

//        return r;
//    }

//    // return a random integer in Z*_n
//    public BigInteger randomZStarN()
//    {
//        BigInteger r;

//        do
//        {
//            r = new BigInteger(modLength, new Random());
//        }
//        while (r.compareTo(n) >= 0 || r.gcd(n).intValue() != 1);

//        return r;
//    }

//    // return a random integer in Z*_{n^2}
//    public BigInteger randomZStarNSquare()
//    {
//        BigInteger r;

//        do
//        {
//            r = new BigInteger(modLength * 2, new Random());
//        }
//        while (r.compareTo(nsquare) >= 0 || r.gcd(nsquare).intValue() != 1);

//        return r;
//    }

//    // return public key as the vector <n, g>
//    public Vector publicKey()
//    {
//        Vector pubKey = new Vector();
//        pubKey.add(n);
//        pubKey.add(g);

//        return pubKey;
//    }
//}
}
