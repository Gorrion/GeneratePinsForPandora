using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Lib
{
    public class MathExt
    {
        public static Point GetPointOnLine(Point a, Point b, int length)
        {
            var rab = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            var k = length / rab;

            int pointX = (int)(a.X + (b.X - a.X) * k);
            int pointY = (int)(a.Y + (b.Y - a.Y) * k);

            return new Point(pointX, pointY);
        }

    }
}
