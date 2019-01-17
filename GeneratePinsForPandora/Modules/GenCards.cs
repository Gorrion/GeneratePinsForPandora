using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using GeneratePinsForPandora.Be;
using GeneratePinsForPandora.Be.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReqSender.Be;
using ReqSender.Lib;

namespace GeneratePinsForPandora.Modules
{

    public class GenCards
    {
        public Dictionary<string, string> REGIONS = new Dictionary<string, string>()
        {
            {"moscow", "Москва"},
            {"sao", "Северный административный округ"},
            {"zao", "Западный административный округ"},
            {"szao", "Северо-Западный административный округ"},
            {"svao", "Северо-Восточный административный округ"},
            {"uvao", "Юго-Восточный административный округ"},
            {"uao", "Южный административный округ"},
            {"uzao", "Юго-Западный административный округ"},
            {"vao", "Восточный административный округ"},
            {"zel", "Зеленоград"},
            {"cao", "Центральный административный округ"},
            {"novomosk", "Новомосковский административный округ"},
            {"troitsk", "Троицкий административный округ"}
        };

        const int SPECIALIZATION_ID = 68;
        const int SHARED_USE_INFRASTRUCTURE_ID = 48;
        const int SOCIAL_INFRASTRUCTURE_ID = 62;
        const int SERVICES_ID = 74;
        const int SERVICES_FOR_RESIDENTS_ID = 10029;
        const int EDUCATIONAL_PROGRAM_ID = 71;

        private static Dictionary<int, DictionaryData> _dictData = new Dictionary<int, DictionaryData>();
        private static void FillDictData(List<DictionaryData> lst, int? parentId = null)
        {
            if (lst == null) return;
            foreach (var d in lst)
            {
                d.ParentId = parentId;
                _dictData[d.Id] = d;

                FillDictData(d.Dictionaries, d.Id);
            }
        }

        public static async Task GenerateAsync()
        {
            var dicts = await GetDictsAsync();
            if (dicts == null) { Console.WriteLine("Ошибка получения словарей"); return; }
            FillDictData(dicts.Data);

            foreach (InnoTypes tp in Enum.GetValues(typeof(InnoTypes)))
            {
                await GenTypeCardsAsync(tp);
            }
        }

        private static async Task GenTypeCardsAsync(InnoTypes tp)
        {
            var typeCargs = await GetInnoTypeInfoAsync((int)tp);
            if (typeCargs == null) return;
            var listTsk = new List<Task<Datum>>();

            foreach (var el in typeCargs.Data)
            {
                if (el.Id != 125330) continue;
                listTsk.Add(DrawCardAsync(el, tp));
                return;
            }

            await Task.WhenAll(listTsk);
        }

        private static async Task<Datum> DrawCardAsync(Datum cardBase, InnoTypes tp)
        {
            try
            {
                var cardResponce = await GetObjDataAsync(cardBase.Id);
                var cardData = cardResponce?.Data;

                if (cardData == null) return null;

                var tpName = Enum.GetName(typeof(InnoTypes), tp).ToLower();
                var bkPath = $"Resource\\Img\\card_back\\imo_{tpName}.png";
                if (!File.Exists(bkPath)) { Console.WriteLine($"Нет ресурсов - бекграунд {tpName}"); }

                using (var bg = Image.FromFile(bkPath))
                {
                    using (Graphics graphics = Graphics.FromImage(bg))
                    {
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                        DrawText(cardData.Name, graphics, 66, 345, 655, 650, 200);

                        DrawText(cardData.Website, graphics, 18, 375, 895, 650, 50);
                        DrawText(cardData.Email, graphics, 18, 375, 920, 650, 50);
                        DrawText(cardData.Phone, graphics, 18, 375, 955, 650, 50);
                        DrawText(cardData.Address, graphics, 18, 375, 1005, 650, 80);

                        var photos = cardData?.Data?.Photos ?? new List<PhotoInfo>();
                        for (var i = 0; i < photos.Count && i < 3; i++)
                        {
                            var img = LoadImg(photos[i].ImageUrl);

                            if (i == 0) graphics.DrawImage(img, 1009, 595, 522, 307);
                            else if (i == 2) graphics.DrawImage(img, 1009 + i * (150 + 186), 936, 150, 100);
                        }

                        var dicts = cardData?.BaseDictionaries ?? new List<BaseDictionary>();
                        var countInRow = 3;
                        var infrastructures = dicts.Where(x => x.DictionaryID == SHARED_USE_INFRASTRUCTURE_ID || x.DictionaryID == SOCIAL_INFRASTRUCTURE_ID).ToList();
                        for (var i = 0; i < infrastructures.Count && i < 6; i++)
                        {
                            var cell = (Math.Abs(i * countInRow) + i) % countInRow;
                            var row = i % countInRow;

                            var el = infrastructures[i];
                            if (_dictData.ContainsKey(el.DictionaryID))
                            {
                                var dictEl = _dictData[el.DictionaryID];
                                var impPath = $"Resource\\Img\\infrastructure_icons\\{dictEl.Name}.png";

                                if (!File.Exists(impPath)) { Console.WriteLine("Не найден файл инфраструктуры"); break; }
                                using (var img = Image.FromFile(impPath))
                                {
                                    graphics.DrawImage(img, 1005 + cell * (155), 1172 + row * (143), img.Width, img.Height);
                                }
                            }
                        }

                        var services = dicts.Where(x => x.DictionaryID == SPECIALIZATION_ID).ToList();
                        for (var i = 0; i < services.Count && i < 5; i++)
                        {
                            var el = infrastructures[i];
                            if (_dictData.ContainsKey(el.DictionaryID))
                            {
                                var dictEl = _dictData[el.DictionaryID];
                                DrawText(". " + dictEl.Name, graphics, 16, 1005, 1468 + i * (26), 650, 30);
                            }
                        }

                        if (tp == InnoTypes.Coworking)
                        {
                            var info = await GetFormatspacesAsync(cardBase.Id);
                            if (info == null) return null;

                            var spaces = info
                                .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.ImgUrl))
                                .OrderBy(x => x.Name)
                                .GroupBy(x => x.Name.IndexOf("(") != -1 ? x.Name.Substring(0, x.Name.IndexOf("(")) : x.Name)
                                .Select(x => x.First())
                                .ToList();

                            for (var i = 0; i < spaces.Count && i < 2; i++)
                            {
                                var sp = spaces[i];
                                var img = LoadImg(sp.ImgUrl);

                                var xStart = 343 + i * (179 + 27);
                                graphics.DrawImage(img, xStart, 1182, 179, 155);
                                SolidBrush blueBrush = new SolidBrush(Color.FromArgb(227, 227, 227));
                                graphics.FillRectangle(blueBrush, new Rectangle(xStart, 1337, 179, 34));

                                DrawText(sp.Name.ToUpper(), graphics, 14, xStart + 10, 1350, 179, 34, Color.FromArgb(24, 62, 73));

                                if (string.IsNullOrWhiteSpace(sp.Price)) continue;

                                DrawText("Стоимость (руб.)", graphics, 24, xStart, 1392, 179, 50);
                                DrawText(sp.Price, graphics, 24, xStart, 1421, 179, 50, Color.FromArgb(245, 152, 45));
                            }
                        }
                    }

                    if (!Directory.Exists("cards")) { Directory.CreateDirectory("cards"); }
                    bg.Save(Path.Combine($"cards\\{cardData.Id}.png")); //save the image file

                }

                return cardData;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        private static string _token = "ej3yNolPszGOzyO5FIJ1pExKktnvtE8N26NnCdua";

        public static async Task<BaseResponce<List<Datum>>> GetInnoTypeInfoAsync(int typeId)
        {
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/inno-objects/?typeid={typeId}&access_token={_token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url, null);
            if (res.Response == null) return null;
            return JsonConvert.DeserializeObject<BaseResponce<List<Datum>>>(res.Response);
        }

        public static async Task<BaseResponce<Datum>> GetObjDataAsync(int objId)
        {
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/inno-objects/{objId}/?&access_token={_token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url, null);
            if (res.Response == null) return null;
            return JsonConvert.DeserializeObject<BaseResponce<Datum>>(res.Response);
        }

        public static async Task<BaseResponce<List<DictionaryData>>> GetDictsAsync()
        {
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/dictionaries/?access_token={_token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url, null);
            if (res.Response == null) return null;
            return JsonConvert.DeserializeObject<BaseResponce<List<DictionaryData>>>(res.Response);
        }

        public static async Task<List<FormatSpace>> GetFormatspacesAsync(int objId)
        {
            var url = $"http://imoscow.mos.ru/ru/infrastructure/object/detail/{objId}/formatspaces";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url, null);
            if (res.Response == null) return null;

            var document = new HtmlParser().ParseDocument(res.Response);
            var spacesNames = document.QuerySelectorAll(".expanding-blocks__item");
            var spacesDscs = document.QuerySelectorAll(".expanding-blocks__descr");

            var result = new List<FormatSpace>();
            for (var i = 0; i < spacesNames.Length; i++)
            {
                if (spacesDscs.Length <= i) break;
                var desc = spacesDscs[i];

                var descDict = spacesDscs[i].QuerySelectorAll("dl")
                    ?.Select(x =>
                    {
                        var val = x.GetElementsByTagName("dd").FirstOrDefault();
                        return new
                        {
                            key = x.GetElementsByTagName("dt").FirstOrDefault()?.InnerHtml?.ToLower(),
                            value = val != null && !val.HasChildNodes ? (object)val.InnerHtml : val.ChildNodes.Select(c => c.Text()).ToArray(),
                        };
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.key))
                    .GroupBy(x => x.key)
                    .ToDictionary(x => x.Key, y => y.First().value);

                var spaceInfo = new FormatSpace
                {
                    Name = spacesNames[i].QuerySelector(".spaces__descr")?.InnerHtml,
                    Description = descDict.ContainsKey("описание") ? descDict["описание"]?.ToString() : null,
                    ImgUrl = spacesDscs[i].QuerySelector(".img-fluid")?.GetAttribute("src"),
                    Price = descDict.ContainsKey("стоимость (руб.)") ? descDict["стоимость (руб.)"] as string : null,
                    SpaceCount = descDict.ContainsKey("количество мест") ? descDict["количество мест"] as string : null,
                    Worksheet = descDict.ContainsKey("рабочие дни") ? descDict["рабочие дни"] as List<string> : null,
                };
                result.Add(spaceInfo);
            }

            return result;
        }

        private static void DrawText(string text, Graphics graphics, int fontSize, int x, int y, int width, int height, Color? color = null)
        {
            var fontColot = color ?? Color.White;
            using (Font font = new Font("/Assets/Resource/Fonts/Muller_Medium.otf#Muller Medium", fontSize))
            using (var foreBrush = new SolidBrush(fontColot))
            {
                RectangleF rectF1 = new RectangleF(x, y, width, height);
                graphics.DrawString(text, font, foreBrush, rectF1);
            }
        }

        private static Image LoadImg(string url)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Bitmap.FromStream(stream);
            }
        }

    }
}