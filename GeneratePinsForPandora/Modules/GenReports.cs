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
using GeneratePinsForPandora.Lib;

namespace GeneratePinsForPandora.Modules
{
    public class GenReports
    {
        public static async Task GenerateAsync(List<Report> data)
        {
            var listTsk = new List<Task>();
            foreach (var d in data)
            {
                listTsk.Add(Task.Factory.StartNew(() => { GeneratePinImg(d); }));
                break;
            }

            await Task.WhenAll(listTsk);
        }

        private static void GeneratePinImg(Report data)
        {
            var genType = "Report";
            var bgPath = string.Join(Path.DirectorySeparatorChar.ToString(),
                new[] {"Resource", "Img", "reports", "bg.png"});

            if (!File.Exists(bgPath))
            {
                Console.WriteLine($"{genType}. Нет ресурсов - бекграунд {bgPath}");
                return;
            }

            using (var bg = Image.FromFile(bgPath))
            {
                using (Graphics graphics = Graphics.FromImage(bg))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    graphics.DrawText(data.Name, 16, 303, 654, color: Color.FromArgb(255, 2, 2));

                    graphics.DrawText(data.ObjectsCount, 32, 593, 635);
                    graphics.DrawText($"1:{data.OneObjectPeopleCount}", 32, 593, 774);
                    graphics.DrawText(data.AvgPrice, 32, 593, 929);

                    graphics.DrawText(data.Population, 32, 1014, 654);
                    graphics.DrawText(data.PopulationDensity, 32, 1342, 654);

                    var sLineX = 1010;
                    var eLineX = 1581;
                    //График 1
                    var sLineY = 849;
                    graphics.DrawText("211К", 11, sLineX, 832, color: Color.FromArgb(88, 88, 90));

                    graphics.DrawLine(new Pen(Color.FromArgb(71, 76, 92), 1),
                        new Point(sLineX, sLineY), new Point(eLineX, sLineY));
                    graphics.DrawLine(new Pen(Color.FromArgb(52, 56, 68), 1),
                        new Point(sLineX, sLineY + 1), new Point(eLineX, sLineY + 1));
                    //Вторая линия
                    sLineY = 890;
                    graphics.DrawLine(new Pen(Color.FromArgb(22, 22, 29), 1),
                        new Point(sLineX, sLineY), new Point(eLineX, sLineY));
                    graphics.DrawLine(new Pen(Color.FromArgb(25, 26, 32), 1),
                        new Point(sLineX, sLineY + 1), new Point(eLineX, sLineY + 1));
                    //Третья линия
                    sLineY = 929;

                    graphics.DrawText("105К", 11, sLineX, 912, color: Color.FromArgb(88, 88, 90));

                    graphics.DrawLine(new Pen(Color.FromArgb(37, 39, 49), 1),
                        new Point(sLineX, sLineY), new Point(eLineX, sLineY));
                    graphics.DrawLine(new Pen(Color.FromArgb(75, 80, 97), 1),
                        new Point(sLineX, sLineY + 1), new Point(eLineX, sLineY + 1));
                    graphics.DrawLine(new Pen(Color.FromArgb(26, 27, 34), 1),
                        new Point(sLineX, sLineY + 2), new Point(eLineX, sLineY + 2));
                    //Предпоследния линия
                    sLineY = 970;
                    graphics.DrawLine(new Pen(Color.FromArgb(27, 28, 35), 1),
                        new Point(sLineX, sLineY), new Point(eLineX, sLineY));
                    graphics.DrawLine(new Pen(Color.FromArgb(19, 20, 26), 1),
                        new Point(sLineX, sLineY + 1), new Point(eLineX, sLineY + 1));
                    //Нижняя линия
                    sLineY = 1010;
                    graphics.DrawLine(new Pen(Color.FromArgb(64, 68, 83), 2),
                        new Point(sLineX, sLineY), new Point(eLineX, sLineY));

                    //График 3 (Кругоыой)
                    var sum = data.GrafC.Sum(); //100%
                    float lastAngle = 0;
                    for (var i = 0; i < data.GrafC.Length; i++)
                    {
                        var procenrt = data.GrafC[i] * 100.0 / sum;

                        var color = GraphicsExt.GetBlendedColor((int)procenrt);

                        var angle = (float) (360f * procenrt / 100) ;
                        graphics.Dyga(new Pen(color, 10), 1300, 1400, 122,  lastAngle, lastAngle + angle);
                        lastAngle = lastAngle + angle;
                        //graphics.DrawCircle(new Pen(Color.FromArgb(33, 81, 255), 10), 1300, 1400, 122);

                    }
                    
                    



                    /*  foreach (var dt in data)
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
                              var pinPoint = new PointD() {X = dt.Lon, Y = dt.Lat}.ConverPoint(segment).FixPoint();
                              graphics.DrawImage(pin, (int) pinPoint.X, (int) pinPoint.Y, pin.Width, pin.Height);
                          }
                      }*/
                }

                if (!Directory.Exists("reports"))
                {
                    Directory.CreateDirectory("reports");
                }

                bg.Save(string.Join(Path.DirectorySeparatorChar.ToString(),
                    new[] {"reports", $"{data.Area}_{data.Type}.png"}));
            }
        }
    }
}