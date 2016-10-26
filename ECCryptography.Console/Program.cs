using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cryptography.ECC;
using Cryptography.Paillier;
using Mpir.NET;

namespace Cryptography.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var keyPaillier = PaillierManager.Default.GenerateKey();

            //var test = PaillierManager.Default.EncryptNumber(1, keyPaillier);
            //var resTest = PaillierManager.Default.DecryptNumber(test, keyPaillier);
            string paillierLog = "";
            StartTimer();
            mpz_t sumPaillier = PaillierManager.Default.EncryptNumber(0, keyPaillier);
            for (var i = 0; i <= 10000; i++)
            {
                var newNumber = PaillierManager.Default.EncryptNumber(i+1, keyPaillier);

                sumPaillier = PaillierManager.Default.Multiply(sumPaillier, newNumber, keyPaillier);

                if (i%100 == 0)
                {
                    var time = StopWatch.Elapsed;
                    paillierLog += $"({i}) - {time.Minutes}:{time.Seconds}.{time.Milliseconds:000}" + Environment.NewLine;
                }
            }
            StopTimer();
            StartTimer();
            var m = PaillierManager.Default.DecryptNumber(sumPaillier, keyPaillier);
            var time2 = StopWatch.Elapsed;
            paillierLog += $"(decrypt) - {time2.Minutes}:{time2.Seconds}.{time2.Milliseconds:000}" + Environment.NewLine;
            StopTimer();
            var file = new FileWriter();
            file.Write(paillierLog);
            
            System.Console.WriteLine(paillierLog);
            //System.Console.WriteLine(m.ToString());
            //System.Console.ReadKey();



            var numbers = new[] { 1, 2, 3 };
            //var sum = numbers.Sum();

            var keyEcc = ECCManager.Default256.GenerateKey();
            var s1 = ECCManager.Default256.EncryptNumber(numbers[0], keyEcc.OpenKey);
            var s2 = ECCManager.Default256.EncryptNumber(numbers[1], keyEcc.OpenKey);
            var s3 = ECCManager.Default256.EncryptNumber(numbers[2], keyEcc.OpenKey);
            var sResRight = s1.Right + s2.Right + s3.Right;
            var sResLeft = s1.Left + s2.Left + s3.Left;
            var res = sResRight - ECCPoint.Multiply(keyEcc.PrivateKey, sResLeft);

            StartTimer();
            List<ECCPoint> rightParts = new List<ECCPoint>(1000);
            List<ECCPoint> leftParts = new List<ECCPoint>(1000);
            ECCPoint sumRight = null;
            ECCPoint sumLeft = null;

            string eccLog = "";

            for (var i = 0; i <= 10000; i++)
            {
                var encryptedNumber = ECCManager.Default256.EncryptNumber(i+1, keyEcc.OpenKey);
                rightParts.Add(encryptedNumber.Right);
                leftParts.Add(encryptedNumber.Left);

                if (i == 2)
                {
                    sumRight = rightParts[0] + rightParts[1];
                    sumLeft = leftParts[0] + leftParts[1];
                }

                if (i > 2)
                {
                    sumRight = sumRight + encryptedNumber.Right;
                    sumLeft = sumLeft + encryptedNumber.Left;
                }

                if (i % 100 == 0)
                {
                    var time = StopWatch.Elapsed;
                    eccLog += $"({i}) - {time.Minutes}:{time.Seconds}.{time.Milliseconds:000}" + Environment.NewLine;
                }
            }
            StopTimer();
            StartTimer();
                var result = sumRight - ECCPoint.Multiply(keyEcc.PrivateKey, sumLeft);
                var time1 = StopWatch.Elapsed;
                eccLog += $"(decrypt) - {time1.Minutes}:{time1.Seconds}.{time1.Milliseconds:000}" + Environment.NewLine;
            StopTimer();

            file.Write(eccLog);
            
            System.Console.WriteLine(eccLog);
            //var s1 = ECCManager.Default.EncryptNumber(numbers[0], keyPairs.OpenKey);
            //var s2 = ECCManager.Default.EncryptNumber(numbers[1], keyPairs.OpenKey);
            //var s3 = ECCManager.Default.EncryptNumber(numbers[2], keyPairs.OpenKey);

            //var sResRight = s1.Right + s2.Right + s3.Right;
            //var sResLeft = s1.Left + s2.Left + s3.Left;

            //var res = sResRight - ECCPoint.Multiply(keyPairs.PrivateKey, sResLeft);
            //var sumPoint = ECCPoint.Multiply(sum, keyPairs.G);
            //if (res == sumPoint)
            //{
            //    System.Console.WriteLine("Correct");
            //}

            var end = System.Console.ReadKey();
        }
        
        static Stopwatch StopWatch = new Stopwatch();
        static void StartTimer()
        {
            StopWatch.Reset();
            StopWatch.Start();
        }

        static void StopTimer()
        {
            StopWatch.Stop();
            var time = StopWatch.Elapsed;
            var timestr = string.Format("{0}:{1}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
            System.Console.WriteLine(timestr);
        }
    }
    
}
