using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using GeneratePinsForPandora.Be;
using GeneratePinsForPandora.Be.Model;
using GeneratePinsForPandora.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReqSender.Be;
using ReqSender.Lib;

namespace GeneratePinsForPandora.Modules
{
    public class GenCards
    {
        private const int SpecializationId = 68;
        private const int SharedUseInfrastructureId = 48;
        private const int SocialInfrastructureId = 62;
        private const int ServicesId = 74;
        private const int ServicesForResidentsId = 10029;
        private const int EducationalProgramId = 71;

        private static readonly Dictionary<int, DictionaryData> DictData = new Dictionary<int, DictionaryData>();

        private static void FillDictData(List<DictionaryData> lst, int? parentId = null)
        {
            if (lst == null) return;
            foreach (var d in lst)
            {
                d.ParentId = parentId;
                DictData[d.Id] = d;

                FillDictData(d.Dictionaries, d.Id);
            }
        }

        public static async Task<List<Datum>> GenerateAsync()
        {
            var dicts = await GetDictsAsync();
            if (dicts == null)
            {
                Console.WriteLine("Ошибка получения словарей");
                return null;
            }

            FillDictData(dicts.Data);

            var allCards = new List<Datum>();
            foreach (InnoTypes tp in Enum.GetValues(typeof(InnoTypes)))
            {
                var cards = await GenTypeCardsAsync(tp);
                allCards.AddRange(cards);
                break;
            }

            return allCards;
        }

        private static async Task<List<Datum>> GenTypeCardsAsync(InnoTypes tp)
        {
            var typeCargs = await GetInnoTypeInfoAsync((int) tp);
            if (typeCargs == null) return null;
            var listTsk = new List<Task<Datum>>();

            foreach (var el in typeCargs.Data)
            {
                //if (el.Id != 125330) continue;
                listTsk.Add(DrawCardAsync(el, tp));
            }

            await Task.WhenAll(listTsk);
            return listTsk.Select(x => x.Result).ToList();
        }

        private static async Task<Datum> DrawCardAsync(Datum cardBase, InnoTypes tp)
        {
            try
            {
                return cardBase;
                
                var cardResponce = await GetObjDataAsync(cardBase.Id);
                var cardData = cardResponce?.Data;

                if (cardData == null) return null;
                if (tp == InnoTypes.Technopark && cardResponce.Data.TechnoparkStatusID != 4702) return null;

                var tpName = Enum.GetName(typeof(InnoTypes), tp).ToLower();
                var bgPath = string.Join(Path.DirectorySeparatorChar.ToString(),
                    new[] {"Resource", "Img", "card_back", $"imo_{tpName}.png"});

                if (!File.Exists(bgPath))
                {
                    Console.WriteLine($"Нет ресурсов - бекграунд {tpName}");
                }

                using (var bg = Image.FromFile(bgPath))
                    //  using (var bmp = new Bitmap(bg.Width, bg.Height, PixelFormat.Format32bppPArgb))
                {
                    using (Graphics graphics = Graphics.FromImage(bg))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        //graphics.DrawImage(bg, 0, 0);
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                        graphics.DrawText(cardData.Name, 42, 345, 655, 650, 200);

                        graphics.DrawText(cardData.Website, 16, 375, 883, 650, 50, fontStyle: FontStyle.Bold);
                        graphics.DrawText(cardData.Email, 16, 375, 919, 650, 50, fontStyle: FontStyle.Bold);
                        graphics.DrawText(cardData.Phone, 16, 375, 951, 650, 50, fontStyle: FontStyle.Bold);
                        graphics.DrawText(cardData.Address, 16, 375, 1005, 650, 80, fontStyle: FontStyle.Bold);

                        var photos = cardData?.Data?.Photos ?? new List<PhotoInfo>();
                        for (var i = 0; i < photos.Count && i < 4; i++)
                        {
                            var img = GraphicsExt.LoadImg(photos[i].ImageUrl);

                            if (i == 0) graphics.DrawImage(img, 1009, 595, 522, 307);
                            else graphics.DrawImage(img, 1009 + (i - 1) * (150 + 36), 936, 150, 100);
                        }

                        var dicts = (cardData?.BaseDictionaries ?? new List<BaseDictionary>())
                            .Where(x => DictData.ContainsKey(x.DictionaryID))
                            .Select(x => DictData[x.DictionaryID])
                            .ToList();

                        var countInRow = 3;
                        {
                            var infrastructures = dicts.Where(x =>
                                    x.ParentId == SharedUseInfrastructureId || x.ParentId == SocialInfrastructureId)
                                .ToList();

                            for (var i = 0; i < infrastructures.Count && i < 6; i++)
                            {
                                var cell = (Math.Abs(i * countInRow) + i) % countInRow;
                                var row = i / countInRow;

                                var el = infrastructures[i];
                                var impPath = string.Join(Path.DirectorySeparatorChar.ToString(),
                                    new[]
                                    {
                                        "Resource", "Img", "infrastructure_icons", $"{el.Name.Replace(" ", "_")}.png"
                                    });

                                if (!File.Exists(impPath))
                                {
                                    Console.WriteLine("Не найден файл инфраструктуры");
                                    break;
                                }

                                using (var img = Image.FromFile(impPath))
                                {
                                    graphics.DrawImage(img, 1005 + cell * (176), 1160 + row * (110));
                                }
                            }
                        }
                        {
                            var specialization = dicts.Where(x => x.ParentId == SpecializationId).ToList();
                            for (var i = 0; i < specialization.Count && i < 5; i++)
                            {
                                var el = specialization[i];
                                graphics.DrawText(el.Name, 11, 644, 1176 + i * (28), 200, 30,
                                    Color.FromArgb(130, 130, 130));
                            }
                        }
                        {
                            var services = dicts.Where(x => x.ParentId == ServicesForResidentsId).OrderBy(x => x.Id)
                                .ToList();
                            for (var i = 0; i < services.Count && i < 5; i++)
                            {
                                if (i == 0)
                                {
                                    graphics.DrawText("Сервисы", 18, 1005, 1430, 200, 30);
                                }

                                var el = services[i];
                                graphics.DrawText("• " + el.Name, 16, 1005, 1469 + i * (25), 600, 30);
                            }
                        }
                        if (tp == InnoTypes.Coworking)
                        {
                            var info = await GetFormatspacesAsync(cardBase.Id);
                            if (info == null) return null;

                            var spaces = info
                                .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.ImgUrl))
                                .OrderBy(x => x.Name.ToLower())
                                .GroupBy(x =>
                                    (x.Name.IndexOf("(") != -1 ? x.Name.Substring(0, x.Name.IndexOf("(")) : x.Name)
                                    .ToLower())
                                .Select(x => x.First())
                                .ToList();

                            for (var i = 0; i < spaces.Count && i < 2; i++)
                            {
                                var sp = spaces[i];
                                var img = GraphicsExt.LoadImg(sp.ImgUrl);

                                var xStart = 343 + i * (240 + 27);
                                graphics.DrawImage(img, xStart, 1182, 240, 156);

                                SolidBrush blueBrush = new SolidBrush(Color.FromArgb(227, 227, 227));
                                graphics.FillRectangle(blueBrush, new Rectangle(xStart, 1337, 240, 34));

                                graphics.DrawText(sp.Name.ToUpper(), 11, xStart + 10, 1345, 240, 34,
                                    Color.FromArgb(24, 62, 73), FontStyle.Bold);

                                if (string.IsNullOrWhiteSpace(sp.Price)) continue;

                                graphics.DrawText("Стоимость (руб.)", 18, xStart, 1388, 240, 50);
                                graphics.DrawText(sp.Price, 24, xStart, 1421, 240, 50, Color.FromArgb(245, 152, 45));
                            }
                        }
                    }

                    if (!Directory.Exists("cards"))
                    {
                        Directory.CreateDirectory("cards");
                    }

                    bg.Save(string.Join(Path.DirectorySeparatorChar.ToString(),
                        new[] {"cards", $"{cardData.Id}.png"}));
                }

                return cardData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        private const string Token = "ej3yNolPszGOzyO5FIJ1pExKktnvtE8N26NnCdua";

        private static async Task<BaseResponce<List<Datum>>> GetInnoTypeInfoAsync(int typeId)
        {
            //var handler = new HttpClientHandler() { UseProxy = false };
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/inno-objects/?typeid={typeId}&access_token={Token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url);
            if (res.Response == null) return null;
            return JsonConvert.DeserializeObject<BaseResponce<List<Datum>>>(res.Response);
        }

        private static async Task<BaseResponce<Datum>> GetObjDataAsync(int objId)
        {
            //var handler = new HttpClientHandler() { UseProxy = false };
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/inno-objects/{objId}/?&access_token={Token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url);
            if (res.Response == null) return null;
            return JsonConvert.DeserializeObject<BaseResponce<Datum>>(res.Response);
        }

        private static async Task<BaseResponce<List<DictionaryData>>> GetDictsAsync()
        {
            //var handler = new HttpClientHandler() { UseProxy = false };
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/dictionaries/?access_token={Token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url);
            if (res.Response == null) return null;
            return JsonConvert.DeserializeObject<BaseResponce<List<DictionaryData>>>(res.Response);
        }

        public static DateTime LastPageReqDate = DateTime.MinValue;

        private static async Task<List<FormatSpace>> GetFormatspacesAsync(int objId)
        {
            //var handler = new HttpClientHandler() { UseProxy = false };
            var url = $"http://imoscow.mos.ru/ru/infrastructure/object/detail/{objId}/formatspaces";
            if ((DateTime.Now - LastPageReqDate).TotalSeconds < 1) await Task.Delay(TimeSpan.FromSeconds(1));
            var res = await ReqController.SendRequestAsync(ReqType.GET, url);
            LastPageReqDate = DateTime.Now;
            //
            if (res.Response == null) return null;

            var parser = new HtmlParser();
            var document = parser.Parse(res.Response);
            var spacesNames = document.QuerySelectorAll(".expanding-blocks__item");
            var spacesDecs = document.QuerySelectorAll(".expanding-blocks__descr");

            var result = new List<FormatSpace>();
            for (var i = 0; i < spacesNames.Length; i++)
            {
                if (spacesDecs.Length <= i) break;
                var desc = spacesDecs[i];

                var descDict = spacesDecs[i].QuerySelectorAll("dl")
                    .Select(x =>
                    {
                        var val = x.GetElementsByTagName("dd").FirstOrDefault();
                        return new
                        {
                            key = x.GetElementsByTagName("dt").FirstOrDefault()?.InnerHtml?.ToLower(),
                            value = val?.InnerHtml ??
                                    "" // val != null && !val.HasChildNodes ? (object)val.InnerHtml : val.ChildNodes.Select(c => c.Text()).ToArray(),
                        };
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.key))
                    .GroupBy(x => x.key)
                    .ToDictionary(x => x.Key, y => y.First().value);

                var spaceInfo = new FormatSpace
                {
                    Name = spacesNames[i].QuerySelector(".spaces__descr")?.InnerHtml,
                    Description = descDict.ContainsKey("описание") ? descDict["описание"]?.ToString() : null,
                    ImgUrl = spacesDecs[i].QuerySelector(".img-fluid")?.GetAttribute("src"),
                    Price = descDict.ContainsKey("стоимость (руб.)") ? descDict["стоимость (руб.)"] as string : null,
                    SpaceCount = descDict.ContainsKey("количество мест") ? descDict["количество мест"] as string : null,
                    //Worksheet = descDict.ContainsKey("рабочие дни") ? descDict["рабочие дни"] as List<string> : null,
                };
                result.Add(spaceInfo);
            }

            return result;
        }


    }
}