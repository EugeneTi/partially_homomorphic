using Mpir.NET;
using System.Numerics;

namespace Cryptography.ECC
{
    public class EllipticCurve
    {
        public mpz_t FieldModule { get; set; }

        public mpz_t A { get; set; }

        public mpz_t B { get; set; }
        
        public EllipticCurve(mpz_t a, mpz_t b, mpz_t fieldModule)
        {
            A = a;
            B = b;
            FieldModule = fieldModule;
        }

        public ECCPoint GetPoint(mpz_t x, mpz_t y)
        {
            return new ECCPoint(x, y, this);
        }

        public GeneratorPoint SetGeneratorPoint(mpz_t x, mpz_t y, mpz_t n)
        {
            return new GeneratorPoint(x, y, n, this);
        }
    }
}
