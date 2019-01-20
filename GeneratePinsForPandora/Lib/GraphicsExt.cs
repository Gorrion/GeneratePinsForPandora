using System;
using System.Drawing;
using System.Net;

namespace GeneratePinsForPandora.Lib
{
    public static class GraphicsExt
    {
        public static void DrawText(this Graphics graphics, int text, int fontSize, int x, int y,
            int width = 2100, int height = 2100,
            Color? color = null, FontStyle? fontStyle = null)
        {
            DrawText(graphics, text.ToString(), fontSize, x, y, width, height, color, fontStyle);
        }

        public static void DrawText(this Graphics graphics, double text, int fontSize, int x, int y,
            int width = 2100, int height = 2100,
            Color? color = null, FontStyle? fontStyle = null)
        {
            DrawText(graphics, text.ToString(), fontSize, x, y, width, height, color, fontStyle);
        }

        public static void DrawText(this Graphics graphics, string text, int fontSize, int x, int y,
            int width = 2100, int height = 2100,
            Color? color = null, FontStyle? fontStyle = null)
        {
            var fontColor = color ?? Color.White;
            using (var font = new Font("/Assets/Resource/Fonts/Muller_Medium.otf#Muller Medium", fontSize,
                fontStyle ?? FontStyle.Regular))
            using (var foreBrush = new SolidBrush(fontColor))
            {
                RectangleF rectF1 = new RectangleF(x, y, width, height);
                graphics.DrawString(text, font, foreBrush, rectF1);
            }
        }

        public static Image LoadImg(string url)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public static void DrawCircle(this Graphics g, Pen pen,
            float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
            float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }
        
        public static void Dyga(this Graphics g, Pen color, float centerX, float centerY, float radius, float angle1, float angle2)
        {
            angle1 = (float)((angle1 / 180) * Math.PI); //
            angle2 = (float)((angle2 / 180) * Math.PI); // переход из градусов в радианы
 
            float koef = (float)(Math.PI * 2 / Math.Abs(angle2 - angle1)); //определение  
            float iterations = (float)Math.Round((2*radius+5) / koef);       //оптимального количества 
            float delta = (angle2 - angle1) / iterations;                           //итераций
 
            float x1 = centerX + radius * (float)Math.Cos(angle1);
            float y1 = centerY - radius * (float)Math.Sin(angle1);
            for (int i = 0; i < iterations; i++)
            {
                angle1 += delta;
                float x2 = centerX + radius * (float)Math.Cos(angle1);
                float y2 = centerY - radius * (float)Math.Sin(angle1);
                g.DrawLine(color, x1, y1, x2, y2);
                x1 = x2;
                y1 = y2;
            }
 
        }
        
        public static Color GetBlendedColor(int percentage)
        {
            if (percentage < 50)
                return Interpolate(Color.Red, Color.Yellow, percentage / 50.0);
            return Interpolate(Color.Blue, Color.Lime, (percentage - 50) / 50.0);
        }

        private static Color Interpolate(Color color1, Color color2, double fraction)
        {
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
        }

        private static double Interpolate(double d1, double d2, double fraction)
        {
            return d1 + (d2 - d1) * fraction;
        }
    }
}