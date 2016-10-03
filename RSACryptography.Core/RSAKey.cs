using System.Numerics;
using Cryptography.Common;
using Mpir.NET;

namespace Cryptography.RSA
{
    public class RSAKey : Key
    {
        public mpz_t PrivateKey { get; set; }

        public mpz_t OpenKey { get; set; }

        public mpz_t P { get; set; }

        public mpz_t PublicPrime { get; set; }

        public RSAKey Copy()
        {
            return new RSAKey {PrivateKey = PrivateKey, OpenKey = OpenKey, P = P, PublicPrime = PublicPrime};
        }
    }
}
