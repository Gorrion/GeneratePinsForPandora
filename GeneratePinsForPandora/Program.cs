using GeneratePinsForPandora.Be.Model;
using GeneratePinsForPandora.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GeneratePinsForPandora.Be;

namespace GeneratePinsForPandora
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                /*var allCards = GenCards.GenerateAsync().Result;

                if (allCards == null) { Console.WriteLine("Ошибка получения данных..."); return;}
                
                SaveFileData(allCards);

                GenPins.GenerateAsync(allCards).Wait();*/

                GenReports.GenerateAsync(new List<Report>()
                {
                    new Report()
                    {
                        Name = "Аэропорт",
                        Area = "aeroport",
                        Type = "innovation",
                        ObjectsCount = 42,
                        OneObjectPeopleCount = 4103,
                        AvgPrice = 245760,
                        Population = 172332,
                        PopulationDensity = 17220,
                        GrafA = new[]
                        {
                            15000, 15000, 15000, 15000, 16000, 18000, 21000, 22000, 22000, 22000, 22000, 21000, 18000,
                            17000, 16000, 15000, 15000
                        },
                        GrafB1 = new [] { 1 },
                        GrafB2 = new [] { 1 },
                        GrafC = new [] { 14.77, 17.05, 7.18, 4.77, 4.77, 2.39, 2.39, 4.77  },
                        
                        
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка формирования файла с технопарками....");
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        private static void SaveFileData(List<Datum> allCards)
        {
            if (allCards == null)
            {
                Console.WriteLine("Ошибка получения данных...");
                return;
            }

            var forFile = allCards.Select(c => new
            {
                id = c.Id.ToString(),
                name = c.Name,
                tp = ((InnoTypes) c.InnoObjectTypeID).ToString().ToLower(),
                distr = Districts.Moscow.Where(d => d.Code?.ToLower() != "moscow")
                    .FirstOrDefault(d => d.IsIn(c))?.Code
            }).ToList();

            File.WriteAllLines("_Tesho.txt",
                forFile.Select(c => string.Join(" // ", new[] {c.distr, c.tp, c.id, c.name})));
        }
    }
}