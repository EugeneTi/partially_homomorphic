using System;
using System.Globalization;
using System.Numerics;
using Mpir.NET;

namespace Cryptography.ECC
{
    public class ECCManager
    {
        private static ECCManager _default;
        public static ECCManager Default
        {
            get
            {
                if (_default != null)
                    return _default;

                var pStr = "68647976601306097149819007990813932172694353001433054093944634591" +
                        "85543183397656052122559640661454554977296311391480858037121987999" +
                        "716643812574028291115057151";
                var aStr = "-3";
                var bStr = "051953eb9618e1c9a1f929a21a0b68540eea2da725b99b315f3b8b489918ef109" +
                            "e156193951ec7e937b1652c0bd3bb1bf073573df883d2c34f1ef451fd46b503f00";
                var xGStr = "0c6858e06b70404e9cd9e3ecb662395b4429c648139053fb521f828af606b4d3d" +
                            "baa14b5e77efe75928fe1dc127a2ffa8de3348b3c1856a429bf97e7e31c2e5bd66";
                var yGStr = "11839296a789a3bc0045c8a5fb42c7d1bd998f54449579b446817afbd17273e66" +
                            "2c97ee72995ef42640c550b9013fad0761353c7086a272c24088be94769fd16650";
                var nStr = "686479766013060971498190079908139321726943530014330540939446345918" +
                            "554318339765539424505774633321719753296399637136332111386476861244" +
                            "0380340372808892707005449";

                var p = new mpz_t(BigInteger.Parse(pStr, NumberStyles.Integer));
                var a = new mpz_t(BigInteger.Parse(aStr, NumberStyles.Integer));
                var b = new mpz_t(BigInteger.Parse(bStr, NumberStyles.HexNumber));

                var curve = new EllipticCurve(a, b, p);
                
                var generator = curve.SetGeneratorPoint(
                    new mpz_t(BigInteger.Parse(xGStr, NumberStyles.HexNumber)),
                    new mpz_t(BigInteger.Parse(yGStr, NumberStyles.HexNumber)),
                    new mpz_t(BigInteger.Parse(nStr, NumberStyles.Integer)));
                
                _default = new ECCManager(curve, generator);

                return _default;
            }
        }

        private static ECCManager _default192;
        public static ECCManager Default192
        {
            get
            {
                if (_default192 != null)
                    return _default192;

                var pStr = "6277101735386680763835789423207666416083908700390324961279";
                var aStr = "-3";
                var bStr = "64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1";
                var xGStr = "188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012";
                var yGStr = "07192b95ffc8da78631011ed6b24cdd573f977a11e794811";
                var nStr = "6277101735386680763835789423176059013767194773182842284081";

                var p = new mpz_t(BigInteger.Parse(pStr, NumberStyles.Integer));
                var a = new mpz_t(BigInteger.Parse(aStr, NumberStyles.Integer));
                var b = new mpz_t(BigInteger.Parse(bStr, NumberStyles.HexNumber));

                var curve = new EllipticCurve(a, b, p);

                var generator = curve.SetGeneratorPoint(
                    new mpz_t(BigInteger.Parse(xGStr, NumberStyles.HexNumber)),
                    new mpz_t(BigInteger.Parse(yGStr, NumberStyles.HexNumber)),
                    new mpz_t(BigInteger.Parse(nStr, NumberStyles.Integer)));

                _default192 = new ECCManager(curve, generator);

                return _default192;
            }
        }

        public EllipticCurve Curve { get; protected set; }

        public GeneratorPoint GeneratorPoint { get; set; }

        public ECCManager(EllipticCurve curve, GeneratorPoint generator)
        {
            Curve = curve;
            GeneratorPoint = generator;
        }

        /// <summary>
        /// Encode number into elliptic curve poin. Encrypt encoded point.
        /// </summary>
        /// <param name="number">Number for encription</param>
        /// <param name="openKey">Open key</param>
        /// <returns>Item1 - tip for decription. Left part of encrypted number
        /// Item2 - encrypted number. Right part</returns>
        public EncriptionResult EncryptNumber(int number, ECCPoint openKey)
        {
            var seancePrivateKey = GeneratePrivateKey(GeneratorPoint.PointDimention);
            var encodedNumber = ECCPoint.Multiply(number, GeneratorPoint);

            var left = ECCPoint.Multiply(seancePrivateKey, GeneratorPoint); //tip for decryption
            var right = encodedNumber + ECCPoint.Multiply(seancePrivateKey, openKey); //encrypted point

            return new EncriptionResult(left, right);
        }

        public byte[] Encrypt(string openText)
        {
            return null;
        }

        public string Decrypt(byte[] closeText)
        {
            return null;
        }

        private mpz_t GeneratePrivateKey(mpz_t demention)
        {
            Random random = new Random();
            var bytes = new byte[64];
            random.NextBytes(bytes);

            var privateKey = new mpz_t(bytes, 0) % demention;
            if (privateKey < 0) privateKey = privateKey * -1;
            return privateKey;
        }

        public ECCKey GenerateKey()
        {
            var key = new ECCKey();
            key.PrivateKey = GeneratePrivateKey(GeneratorPoint.PointDimention);
            key.OpenKey = ECCPoint.Multiply(key.PrivateKey, GeneratorPoint);

            key.P = GeneratorPoint.Curve.FieldModule;
            key.G = GeneratorPoint;

            return key;
        }

        public ECCPoint CreatePoint()
        {
            Random random = new Random();
            var bytes = new byte[64];
            random.NextBytes(bytes);

            var privateKey = new mpz_t(bytes, 0) % GeneratorPoint.PointDimention;
            if (privateKey < 0) privateKey = privateKey * -1;
            return ECCPoint.Multiply(privateKey, GeneratorPoint);
        }
    }

    public class EncriptionResult
    {
        /// <summary>
        /// Tip for decription
        /// </summary>
        public ECCPoint Left { get; set; }

        /// <summary>
        /// Encrypted number
        /// </summary>
        public ECCPoint Right { get; set; }

        public EncriptionResult(ECCPoint left, ECCPoint right)
        {
            Left = left;
            Right = right;
        }
    }
}
