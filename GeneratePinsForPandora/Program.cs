using GeneratePinsForPandora.Modules;
using System;
using System.Diagnostics;

namespace GeneratePinsForPandora
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            GenCards.GenerateAsync().Wait();

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

        }
    }
}