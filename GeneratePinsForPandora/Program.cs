using GeneratePinsForPandora.Modules;

namespace GeneratePinsForPandora
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            GenCards.GenerateAsync().Wait();

        }
    }
}