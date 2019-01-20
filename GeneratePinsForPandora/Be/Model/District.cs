using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class District
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsMoscow { get; set; }
        public int Level { get; set; }
        public PointD[][] Areas { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }

        public bool IsIn(Datum dt)
        {
            if (dt.Id == 180629 && Id == 446092) return true;

            double x = dt.Lon, y = dt.Lat;
            var isInside = false;

            foreach (var area in Areas)
            {
                for (int i = 0, j = area.Length - 1; i < area.Length; j = i++)
                {
                    double xi = area[i].X, yi = area[i].Y;
                    double xj = area[j].X, yj = area[j].Y;

                    var intersect = ((yi > y) != (yj > y))
                                    && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                    if (intersect) isInside = !isInside;
                }
                if (isInside) return true;
            }

            return false;
        }
        
        public Segment GetSegment()
        {
            var segment = new Segment()
            {
                X = new PointInfo {Min = 90},
                Y = new PointInfo {Min = 90},
                District = this,
            };

            foreach (var area in Areas)
            {
                foreach (var point in area)
                {
                    if (point.X < segment.X.Min)
                    {
                        segment.X.Min = point.X;
                    }
                    if (point.X > segment.X.Max) {
                        segment.X.Max = point.X;
                    }
                    if (point.Y < segment.Y.Min)
                    {
                        segment.Y.Min = point.Y;
                    }
                    if (point.Y > segment.Y.Max)
                    {
                        segment.Y.Max = point.Y;
                    }
                }
            }

            segment.X.Delta = segment.X.Max - segment.X.Min;
            segment.Y.Delta = segment.Y.Max - segment.Y.Min;

            segment.X.D = segment.District.Width / segment.X.Delta;
            segment.Y.D = segment.District.Height / segment.Y.Delta;

            return segment;
        }
    }

    public static class Districts
    {
        static Districts()
        {
            var regionsJson = JObject.Parse(Resource.DtaResource.Regions);
            foreach (var reg in regionsJson)
            {
                var dist = Moscow.FirstOrDefault(d => d.Id.ToString() == reg.Key?.ToString());
                if (dist != null && reg.Value?["raw_coordinates"] is JArray)
                {
                    dist.Areas = reg.Value["raw_coordinates"].OfType<JArray>()
                        .Select(x => x.OfType<JArray>()
                            .Where(p => p.Count() == 2)
                            .Select(p =>
                            {
                                double xP, yP;
                                if (double.TryParse(p[0]?.ToString(), out xP) &&
                                    double.TryParse(p[1]?.ToString(), out yP))
                                {
                                    return new PointD {X = xP, Y = yP};
                                }

                                return new PointD();
                            })
                            .ToArray())
                        .ToArray();

                }
            }
        }

        public static List<District> Moscow = new List<District>
        {
            new District
            {
                Id = 102269,
                Level = 4,
                Name = "Москва",
                Code = "moscow",
                IsMoscow = true,
                Width = 1780, Height = 2380, Top = 220, Left = 130
            },

            new District
            {
                Id = 2263059,
                ParentId = 102269,
                Level = 5,
                Name = "Троицкий административный округ",
                Code = "troitsk",
                Width = 1050, Height = 1200, Top = 495, Left = 835,
            },

            new District
            {
                Id = 2263058,
                ParentId = 102269,
                Level = 5,
                Name = "Новомосковский административный округ",
                Code = "novomosk",
                Width = 1235, Height = 956, Top = 690, Left = 645,
            },

            new District
            {
                Id = 226149,
                ParentId = 102269,
                Level = 5,
                Name = "Западный административный округ",
                Code = "zao",
                Width = 1710, Height = 1770, Top = 270, Left = 205
            },

            new District
            {
                Id = 1320234,
                ParentId = 102269,
                Level = 5,
                Name = "Восточный административный округ",
                Code = "vao",
                Width = 870, Height = 1560, Top = -120, Left = 900,
            },

            new District
            {
                Id = 1278703,
                ParentId = 102269,
                Level = 5,
                Name = "Юго-Восточный административный округ",
                Code = "uvao",
                Width = 1300, Height = 1130, Top = 530, Left = 970,
            },

            new District
            {
                Id = 162903,
                ParentId = 102269,
                Level = 5,
                Name = "Северный административный округ",
                Code = "sao",
                Width = 1385, Height = 1920, Top = -10, Left = 258
            },

            new District
            {
                Id = 1252558,
                ParentId = 102269,
                Level = 5,
                Name = "Северо-Восточный административный округ",
                Code = "svao",
                Width = 887, Height = 1290, Top = 385, Left = 903
            },

            new District
            {
                Id = 1282181,
                ParentId = 102269,
                Level = 5,
                Name = "Южный административный округ",
                Code = "uao",
                Width = 960, Height = 1230, Top = 465, Left = 925,
            },

            new District
            {
                Id = 1304596,
                ParentId = 102269,
                Level = 5,
                Name = "Юго-Западный административный округ",
                Code = "uzao",
                Width = 550, Height = 1330, Top = 460, Left = 970,
            },

            new District
            {
                Id = 446092,
                ParentId = 102269,
                Level = 5,
                Name = "Северо-Западный административный округ",
                Code = "szao",
                Width = 690, Height = 1114, Top = 670, Left = 1000
            },

            new District
            {
                Id = 2162196,
                ParentId = 102269,
                Level = 5,
                Name = "Центральный административный округ",
                Code = "cao",
                Width = 1160, Height = 905, Top = 660, Left = 720,
            },

            new District
            {
                Id = 1320358,
                ParentId = 102269,
                Level = 5,
                Name = "Зеленоград",
                Code = "zel",
                Width = 0, Height = 0, Top = 0, Left = 0,
            },
        };
    }
}