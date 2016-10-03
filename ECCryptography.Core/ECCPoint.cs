using System;
using System.Numerics;
using System.Threading.Tasks;
using Mpir.NET;

namespace Cryptography.ECC
{
    public class ECCPoint
    {
        public static ECCPoint O = new ECCPoint();

        public mpz_t X { get; set; }

        public mpz_t Y { get; set; }

        public bool IsInf { get; set; }

        public EllipticCurve Curve { get; set; }

        protected ECCPoint()
        {
            IsInf = true;
        }

        public ECCPoint(mpz_t x, mpz_t y, EllipticCurve curve)
        {
            X = x;
            Y = y;
            Curve = curve;
        }

        public static ECCPoint operator +(ECCPoint first, ECCPoint second)
        {
            if (first == O)
                return second;
            if (second == O)
                return first;

            var curve = first.Curve;

            var firstX = new mpz_t(first.X);
            var firstY = new mpz_t(first.Y);
            var secondX = new mpz_t(second.X);
            var secondY = new mpz_t(second.Y);
            var module = new mpz_t(curve.FieldModule);
            //ECCPoint result = curve.GetPoint(first.X, first.Y);
            
            var dy = secondY - firstY;
            var dx = secondX - firstX;

            if (dx == 0)
                return O;

            if (dx < 0)
                dx += module;
            if (dy < 0)
                dy += module;

            var dxInverse = dx.InvertMod(module);// BigInteger.ModPow(dx, curve.FieldModule - 2, curve.FieldModule);
            var m = (dy * dxInverse) % module;

            if (m < 0)
                m += module;
            var resultX = (m * m - firstX - secondX) % module;
            var resultY = (m * (firstX - resultX) - firstY) % module;
            if (resultX < 0)
                resultX += module;
            if (resultY < 0)
                resultY += module;
            
            return curve.GetPoint(resultX, resultY);
        }

        public static ECCPoint operator -(ECCPoint first, ECCPoint second)
        {
            second.Y = -1*second.Y;
            if (first == O)
                return second;
            if (second == O)
                return first;

            var curve = first.Curve;

            var firstX = new mpz_t(first.X);
            var firstY = new mpz_t(first.Y);
            var secondX = new mpz_t(second.X);
            var secondY = new mpz_t(second.Y);
            var module = new mpz_t(curve.FieldModule);
            //ECCPoint result = curve.GetPoint(first.X, first.Y);

            var dy = secondY - firstY;
            var dx = secondX - firstX;

            if (dx == 0)
                return O;

            if (dx < 0)
                dx += module;
            if (dy < 0)
                dy += module;

            var dxInverse = dx.InvertMod(module);// BigInteger.ModPow(dx, curve.FieldModule - 2, curve.FieldModule);
            var m = (dy * dxInverse) % module;

            if (m < 0)
                m += module;
            var resultX = (m * m - firstX - secondX) % module;
            var resultY = (m * (firstX - resultX) - firstY) % module;
            if (resultX < 0)
                resultX += module;
            if (resultY < 0)
                resultY += module;

            return curve.GetPoint(resultX, resultY);
        }

        public static ECCPoint Double(ECCPoint point)
        {
            var pointX = new mpz_t(point.X);
            var pointY = new mpz_t(point.Y);
            var module = new mpz_t(point.Curve.FieldModule);
            var curveAkof = new mpz_t(point.Curve.A);

            if (point == O)
                return O;
            
            var dy = 3 * pointX * pointX + curveAkof;
            var dx = 2 * pointY;

            if (dx < 0)
                dx += module;
            if (dy < 0)
                dy += module;

            var dxInverse = dx.InvertMod(module);// BigInteger.ModPow(dx, curve.FieldModule - 2, curve.FieldModule);
            var m = (dy * dxInverse) % module;

            var resultX = (m * m - pointX - pointX) % module;
            var resultY = (m * (pointX - resultX) - pointY) % module;
            if (resultX < 0)
                resultX += module;
            if (resultY < 0)
                resultY += module;

            return point.Curve.GetPoint(resultX, resultY);
        }

        public static ECCPoint Multiply(mpz_t x, ECCPoint p)
        {
            ECCPoint temp = p;
           
            x = x - 1;
            while (x != 0)
            {

                if ((x % 2) != 0)
                {
                    if ((temp.X == p.X) || (temp.Y == p.Y))
                        temp = Double(temp);
                    else
                        temp = temp + p;
                    x = x - 1;
                }
                x = x / 2;
                p = Double(p);
            }
            return temp;
        }

        public static bool operator ==(ECCPoint first, ECCPoint second)
        {
            if (ReferenceEquals(first, second))
                return true;

            if (ReferenceEquals(first, null))
                return false;

            if (ReferenceEquals(second, null))
                return false;

            if (first.IsInf && second.IsInf)
                return true;

            return first.X == second.X && first.Y == second.Y;
        }

        public static bool operator !=(ECCPoint first, ECCPoint second)
        {
            return !(first == second);
        }

        public override string ToString()
        {
            if (IsInf) return "Infinity";
            return $"({X}, {Y})";
        }
    }

    public class GeneratorPoint : ECCPoint
    {
        public mpz_t PointDimention { get; set; }

        public GeneratorPoint(mpz_t x, mpz_t y, mpz_t dimention, EllipticCurve curve) : base(x, y, curve)
        {
            PointDimention = dimention;
        }
    }
}
