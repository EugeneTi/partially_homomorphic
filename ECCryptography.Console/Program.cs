using System;
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

            var keyPairs = ECCManager.Default.GenerateKey();
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
    }
    
}
