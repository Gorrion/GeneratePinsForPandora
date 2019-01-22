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
using System.Drawing.Drawing2D;

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
                new[] { "Resource", "Img", "reports", "bg.png" });

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

                    graphics.DrawText(data.Population, 32, 1000, 635);
                    graphics.DrawText(data.PopulationDensity, 32, 1342, 635);
                    {
                        var sLineX = 1010;
                        var eLineX = 1581;
                        //График 1

                        var max = data.GrafA.Max();
                        var avg = data.GrafA.Average();

                        var nearMax = Math.Round(max - max * 0.005);
                        var nearMaxStr = nearMax < 1000 ? nearMax.ToString() : Math.Round(nearMax / 1000) + "K";

                        var sLineY = 849;
                        graphics.DrawText(nearMaxStr, 11, sLineX, 832, color: Color.FromArgb(88, 88, 90));

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

                        var halfNearMax = nearMax / 2;
                        var halfNearMaxStr = halfNearMax < 1000 ? halfNearMax.ToString() : (halfNearMax / 1000).ToString("N1") + "K";
                        graphics.DrawText(halfNearMaxStr, 11, sLineX, 912, color: Color.FromArgb(88, 88, 90));

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

                        //Строим столбцы
                        var eLineY = 1000;
                        var grafHeight = eLineY - 840;
                        var grafWidth = eLineX - sLineX + 5 - 20;
                        // var grafHeight = 100;
                        if (data.GrafA.Length > 0)
                        {
                            var pdx = grafWidth / data.GrafA.Length;
                            for (var i = 0; i < data.GrafA.Length; i++)
                            {
                                var dy = (int)Math.Round(grafHeight * (data.GrafA[i] * 100.0 / max) / 100.0);
                                var stX = sLineX + 5 + i * 34;

                                LinearGradientBrush linGrBrush = new LinearGradientBrush(
                                   new Point(stX, eLineY - dy - 10),
                                   new Point(stX, eLineY + 10),
                                   Color.FromArgb(100, 255, 255, 255), 
                                   Color.FromArgb(0, 255, 255, 255));  

                                Pen pen = new Pen(linGrBrush, 6);
                                graphics.DrawLine(pen, new Point(stX, eLineY - dy), new Point(stX, eLineY));

                                var b = new SolidBrush(Color.FromArgb(50, 154, 216));
                                graphics.FillRectangle(b, stX - 3, eLineY - dy, 6, 6);
                            }
                        }
                    }

                    //График 2
                    {
                        var max = (data.GrafB1.Length > 0 ? data.GrafB1.Max() : 0) +
                                  (data.GrafB2.Length > 0 ? data.GrafB2.Max() : 0);

                        
                        graphics.DrawText("211К", 10, 722, 1235, color: Color.Gray);
                        graphics.DrawText("141К", 10, 722, 1284, color: Color.Gray);
                        graphics.DrawText("70К", 10, 722, 1318, color: Color.Gray);

                        
                        var center = new Point(717, 1392);
                        var angCount = 360 / 24;
                        var radius = 160;

                        var pointsWhite = data.GrafB1.Select((x, i) =>
                        {
                            var ang = angCount * i;
                            var proc = (x * 100.0 / max) / 100 * radius;
                            

                            double angle2 = Math.PI * ang / 180.0;
                            var txtX = (int)(radius * Math.Sin(angle2));
                            var txtY = (int)(radius * Math.Cos(angle2));
                            var pointOnCircle = new Point(center.X + txtX, center.Y - txtY);

                            return MathExt.GetPointOnLine(center, pointOnCircle, (int)proc);
                        });
                        
                        graphics.DrawPolygon(new Pen(Color.White, 2), pointsWhite.ToArray());
                        
                        var pointsRed = data.GrafB2.Select((x, i) =>
                        {
                            var ang = angCount * i;
                            var proc = (x * 100.0 / max) / 100 * radius;
                            

                            double angle2 = Math.PI * ang / 180.0;
                            var txtX = (int)(radius * Math.Sin(angle2));
                            var txtY = (int)(radius * Math.Cos(angle2));
                            var pointOnCircle = new Point(center.X + txtX, center.Y - txtY);

                            return MathExt.GetPointOnLine(center, pointOnCircle, (int)proc);
                        });
                        
                        graphics.DrawPolygon(new Pen(Color.Red, 2), pointsRed.ToArray());
                        
                    }


                    //График 3 (Кругоыой)
                    {
                        var topLeftX = 1170;
                        var topLeftY = 1270;
                        var width = 260;
                        var radius = width / 2;

                        var centr = new Point(topLeftX + width / 2, topLeftY + width / 2);

                        graphics.DrawText("ОБЪЕКТЫ", 16,
                            topLeftX + width / 2 - 20 * 3, topLeftY + width / 2 - 10,
                            color: Color.FromArgb(217, 217, 217));

                        var sum = data.GrafC.Sum(); //100%
                        var colorArr = new[] {
                        Color.FromArgb(255, 46,46),
                        Color.FromArgb(255, 69,69),
                        Color.FromArgb(255,163,163),
                        Color.FromArgb(255, 212,212),
                        Color.FromArgb(33,81,255),
                        Color.FromArgb(46,147,255),
                        Color.FromArgb(130,130,141),
                        Color.FromArgb(227, 240, 255),
                    };


                        float lastAngle = 0 - 90;
                        var angleDx = 0;
                        for (var i = 0; i < data.GrafC.Length; i++)
                        {
                            var colorInd = (Math.Abs(i * colorArr.Length) + i) % colorArr.Length;
                            var color = colorArr[colorInd];//GraphicsExt.GetBlendedColor((int)procenrt);

                            var procenrt = data.GrafC[i] * 100.0 / sum;

                            var angle = (float)(360f * procenrt / 100);

                            //graphics.Dyga(new Pen(color, 10), 1300, 1400, 122,  lastAngle, lastAngle + angle);

                            graphics.DrawArc(new Pen(color, 12), topLeftX, topLeftY, width, width, lastAngle, angle);

                            //Подпись процентов
                            //var txtX = (int)Math.Floor(radius * Math.Sin(lastAngle + 90));
                            //var txtY = (int)Math.Floor(radius * Math.Cos(lastAngle + 90));
                            //// graphics.DrawText(procenrt.ToString(), 12, topLeftX + txtX, topLeftY + txtY, color: Color.Red);
                            //graphics.DrawLine(new Pen(color, 1), centr, new Point(centr.X - txtX, centr.Y - txtY));
                            {
                                var angleTxt = lastAngle + 90 + angle / 10;

                                double angle2 = Math.PI * angleTxt / 180.0;
                                var txtX = (int)(radius * Math.Sin(angle2));
                                var txtY = (int)(radius * Math.Cos(angle2));

                                //var txtX = Math.Sin((0 / 180D) * Math.PI); // (int)Math.Floor(radius * Math.Sin(0));
                                //var txtY = (int)Math.Floor(radius * Math.Cos(0));
                                // graphics.DrawText(procenrt.ToString(), 12, topLeftX + txtX, topLeftY + txtY, color: Color.Red);
                                var pointOnCircle = new Point(centr.X + txtX, centr.Y - txtY);
                                graphics.DrawLine(new Pen(color, 1), centr, pointOnCircle);

                                //   if (angle > 20) angleDx = 0;

                                var txtPoint = MathExt.GetPointOnLine(centr, pointOnCircle, radius + (angleTxt < 180 ? 25 : 50)); // (angle i % 2 == 0 ? 15 : 0));

                                StringFormat format = new StringFormat();
                                format.LineAlignment = StringAlignment.Near;

                                if (angleTxt < 180)
                                {
                                    format.Alignment = StringAlignment.Near;
                                    graphics.DrawText(procenrt.ToString("N2"), 12, txtPoint.X, txtPoint.Y - 16 - (angleTxt < 30 && i > 0 && i % 2 == 0 ? 20 : 0), 200, 60, color: Color.Red);

                                    var title = data.GrafCT?[i];
                                    if (!string.IsNullOrWhiteSpace(title))
                                    {


                                        graphics.DrawText(title, 8, txtPoint.X, txtPoint.Y, 150, 60, color: Color.FromArgb(65, 65, 65), format: format);
                                    }


                                }
                                else
                                {
                                    format.Alignment = StringAlignment.Near;
                                }




                                // if (angle < 20) angleDx += 15;

                            }


                            //{
                            //    var txtX = (int)Math.Floor(radius * Math.Cos(90));
                            //    var txtY = (int)Math.Floor(radius * Math.Sin(90));
                            //    // graphics.DrawText(procenrt.ToString(), 12, topLeftX + txtX, topLeftY + txtY, color: Color.Red);
                            //    graphics.DrawLine(new Pen(color, 1), centr, new Point(centr.X + txtX, centr.Y - txtY));
                            //}

                            //{
                            //    var txtX = (int)Math.Floor(radius * Math.Cos(180));
                            //    var txtY = (int)Math.Floor(radius * Math.Sin(180));
                            //    // graphics.DrawText(procenrt.ToString(), 12, topLeftX + txtX, topLeftY + txtY, color: Color.Red);
                            //    graphics.DrawLine(new Pen(color, 1), centr, new Point(centr.X + txtX, centr.Y - txtY));
                            //}

                            lastAngle = lastAngle + angle;
                            //graphics.DrawCircle(new Pen(Color.FromArgb(33, 81, 255), 10), 1300, 1400, 122);

                        }

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
                    new[] { "reports", $"{data.Area}_{data.Type}.png" }));
            }
        }
    }
}