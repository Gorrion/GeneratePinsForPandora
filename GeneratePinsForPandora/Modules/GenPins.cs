using GeneratePinsForPandora.Be;
using GeneratePinsForPandora.Be.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Modules
{
    public class GenPins
    {
        public static async Task GenerateAsync(List<Datum> data)
        {
            var listTsk = new List<Task>();
            foreach (var dist in Districts.Moscow)
            {
                foreach (InnoTypes tp in Enum.GetValues(typeof(InnoTypes)))
                {
                    var cards = data.Where(c => c.InnoObjectTypeID == (int) tp && dist.IsIn(c));
                    listTsk.Add(Task.Factory.StartNew(() => { GeneratePinImg(dist, cards.ToArray()); }));
                    break;
                }
            }

            await Task.WhenAll(listTsk);
        }

        private static void GeneratePinImg(District dist, IEnumerable<Datum> data)
        {
            var genType = "Pins";
            var bgPath = string.Join(Path.DirectorySeparatorChar.ToString(),
                new[] {"Resource", "Img", "districts", $"{dist.Code.ToLower()}.png"});

            if (!File.Exists(bgPath))
            {
                Console.WriteLine($"{genType}. Нет ресурсов - бекграунд {bgPath}");
                return;
            }

            var segment = dist.GetSegment();

            var drawBg = false;
            
            using (var bg = drawBg ? Image.FromFile(bgPath) : new Bitmap(1920, 2160))
            {
                using (Graphics graphics = Graphics.FromImage(bg))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    foreach (var dt in data)
                    {
                        var dtType = (InnoTypes) dt.InnoObjectTypeID;
                        var pinTypePath = string.Join(Path.DirectorySeparatorChar.ToString(),
                            new[] {"Resource", "Img", "pins", $"{dtType.ToString().ToLower()}.png"});
                        
                        if (!File.Exists(pinTypePath))
                        {
                            Console.WriteLine($"{genType}. Нет ресурсов - бекграунд {pinTypePath}");
                            continue;
                        }
                        
                        using (var pin = Image.FromFile(pinTypePath))
                        {
                            var pinPoint = new PointD() { X = dt.Lon, Y = dt.Lat}.ConverPoint(segment).FixPoint();
                            graphics.DrawImage(pin, (int)pinPoint.X, (int)pinPoint.Y, pin.Width, pin.Height);
                        }
                    }
                }

                if (!Directory.Exists("pins"))
                {
                    Directory.CreateDirectory("pins");
                }

                var innoTypes = data.GroupBy(t => t.InnoObjectTypeID).Select(t => (InnoTypes) t.Key).ToArray();
                var fileName = dist.Code + (innoTypes.Length == 1 ? "_" + innoTypes[0].ToString().ToLower() : string.Empty) + ".png";
                bg.Save(string.Join(Path.DirectorySeparatorChar.ToString(),
                    new[] {"pins", fileName}));
            }
        }
    }
}