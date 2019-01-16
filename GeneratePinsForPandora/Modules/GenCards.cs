using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ReqSender.Be;
using ReqSender.Lib;

namespace GeneratePinsForPandora.Modules
{
    public enum INNO_TYPES
    {
        Coworking = 6706,
        Cmiit = 6717,
        Special = 6710,
        Kids = 6723,
        Technopark = 6712
    }

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

        public static void Generate()
        {
           /* var fontSize = row.Name == row.Name.ToUpper() ? 18 : 20;
            var source = Image.FromFile(Path.Combine(folderPath, row.Folder, "Cover.png"));
            using (Graphics graphics = Graphics.FromImage(mainBitmap))
            {
                var ratioX = (double) imgItemWidth / source.Width;
                var ratioY = (double) imgItemHeight / source.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int) (source.Width * ratio);
                var newHeight = (int) (source.Height * ratio);

                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                graphics.DrawImage(source, leftX, 0, newWidth, newHeight);

                using (Font font = new Font("/Assets/Fonts/Muller_Medium.otf#Muller Medium", fontSize)
                ) //"/Assets/Fonts/Muller_Medium.otf#Muller Medium"
                using (var foreBrush = new SolidBrush(Color.White))
                {
                    //font.Size = 
                    //using (Font arialFont = new Font("Arial", 20))
                    {
                        RectangleF rectF1 = new RectangleF(leftX, imgItemHeight + 50, imgItemWidth,
                            mainBitmap.Height);
                        //graphics.DrawString(firstText, arialFont, Brushes.Blue, firstLocation);
                        //graphics.DrawString(secondText, arialFont, Brushes.Red, secondLocation);
                        graphics.DrawString(row.Name, font, foreBrush, rectF1);
                        //graphics.DrawString(secondText, arialFont, Brushes.Red, rectF1);

                        //Pen blackPen = new Pen(Color.Black, 3);

                        //// Create points that define line.
                        //Point point1 = new Point(leftX, mainBitmap.Height - 3);
                        //Point point2 = new Point(rightX, mainBitmap.Height - 3);

                        //// Draw line to screen.
                        //graphics.DrawLine(blackPen, point1, point2);
                    }
                }
            }*/
           
           
           foreach (var name in Enum.GetValues(typeof(INNO_TYPES)))
           {
           //    var objs = 
              
           }
         
        }

        private static async Task GenTypeCards(INNO_TYPES tp)
        {
            var typeCargs = await GetObjsUrl((int)tp);
            if (typeCargs == null && typeCargs["data"] is JArray) return;
            var listTsk = new List<Task<JObject>>();
            
            
        }

        private static async Task DrawCard(INNO_TYPES tp)
        {
        }

        private static string _token = "ej3yNolPszGOzyO5FIJ1pExKktnvtE8N26NnCdua";
        
        public static async Task<JObject> GetObjsUrl(int typeId)
        {
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/inno-objects/?typeid={typeId}&access_token={_token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url, null);
            return JObject.Parse(res?.Response);
        }
        
        public static async Task<JObject> GetObjUrl(int objId)
        {
            var url = $"https://iasdnpp.mos.ru/dnpp_map/api/inno-objects/{objId}/?&access_token={_token}";
            var res = await ReqController.SendRequestAsync(ReqType.GET, url, null);
            return JObject.Parse(res?.Response);
        }

    }
}