using System.Numerics;
using Cryptography.Common;
using Mpir.NET;

namespace Cryptography.ECC
{
    public class ECCKey : Key
    {
        public mpz_t PrivateKey { get; set; }

        public ECCPoint OpenKey { get; set; }

        public ECCPoint G { get; set; }

        public mpz_t P { get; set; }

        public ECCKey Copy()
        {
            return new ECCKey
            {
                G = G,
                PrivateKey = PrivateKey,
                OpenKey = OpenKey,
                P = P
            };
        }
    }
}
