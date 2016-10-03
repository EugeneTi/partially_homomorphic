using System;
using System.Collections.Generic;
using Cryptography.Common;
using System.Diagnostics;
using System.Linq;
using Cryptography.ECC;
using Mpir.NET;

namespace Cryptography.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = new []{1, 2, 3};
            var sum = numbers.Sum();

            var manager = ECCManager.Default192;

            var keyPairs = (ECCKey)ECCManager.Default.GenerateKey();
            var s1 = ECCManager.Default.EncryptNumber(numbers[0], keyPairs.OpenKey);
            var s2 = ECCManager.Default.EncryptNumber(numbers[1], keyPairs.OpenKey);
            var s3 = ECCManager.Default.EncryptNumber(numbers[2], keyPairs.OpenKey);

            var sResRight = s1.Right + s2.Right + s3.Right;
            var sResLeft = s1.Left + s2.Left + s3.Left;

            var res = sResRight - ECCPoint.Multiply(keyPairs.PrivateKey, sResLeft);
            var sumPoint = ECCPoint.Multiply(sum, keyPairs.G);
            if (res == sumPoint)
            {
                System.Console.WriteLine("Correct");
            }

            var end = System.Console.ReadKey();
        }

        static void Test(Algorithm algorithm, int personCount)
        {
            var persons = new List<Person>();
            for (int i = 0; i < personCount; i++)
            {
                var person = new Person(algorithm);
                person.GenerateKeys();

                persons.Add(person);
            }
    
            InitTimer(string.Format("{0}. Count: {1}. Time: ", algorithm, personCount));
            persons[0].GenerateSymetricKey(persons);
            GetTime();
        }

        static Stopwatch StopWatch = new Stopwatch();
        static void InitTimer(string time)
        {
            System.Console.Write(time);
            StopWatch.Reset();
            StopWatch.Start();
        }

        static void GetTime()
        {
            StopWatch.Stop();
            var time = StopWatch.Elapsed;
            var timestr = string.Format("{0}:{1}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
            System.Console.WriteLine(timestr);
        }

        private static mpz_t GeneratePrivateKey(mpz_t demention)
        {
            Random random = new Random();
            var bytes = new byte[64];
            random.NextBytes(bytes);

            var privateKey = new mpz_t(bytes, 0) % demention;
            if (privateKey < 0) privateKey = privateKey * -1;
            return privateKey;
        }
    }

    public static class EncriptionHelper
    {
        public static EncriptionResult EncryptNumber(this ECCManager manager, int number, ECCPoint openKey)
        {
            var res = new EncriptionResult();
            res.Number = number;
            res.Key = GeneratePrivateKey(manager.GeneratorPoint.PointDimention);
            res.EncodedNumber = ECCPoint.Multiply(number, manager.GeneratorPoint);
            res.Left = ECCPoint.Multiply(res.Key, manager.GeneratorPoint);
            res.Right = res.EncodedNumber + ECCPoint.Multiply(res.Key, openKey);

            return res;
        }
        
        private static mpz_t GeneratePrivateKey(mpz_t demention)
        {
            Random random = new Random();
            var bytes = new byte[64];
            random.NextBytes(bytes);

            var privateKey = new mpz_t(bytes, 0) % demention;
            if (privateKey < 0) privateKey = privateKey * -1;
            return privateKey;
        }
    }

    public class EncriptionResult
    {
        public int Number { get; set; }
        public mpz_t Key { get; set; }
        public ECCPoint EncodedNumber { get; set; }
        public ECCPoint Left { get; set; }
        public ECCPoint Right { get; set; }
    }
}
