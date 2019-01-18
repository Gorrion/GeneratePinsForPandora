using GeneratePinsForPandora.Be.Model;
using GeneratePinsForPandora.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;

namespace GeneratePinsForPandora
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var tt = Districts.Moscow;


           // GenCards.GenerateAsync().Wait();

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

        }
    }
}