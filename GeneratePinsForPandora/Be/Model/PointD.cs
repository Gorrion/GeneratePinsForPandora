namespace GeneratePinsForPandora.Be.Model
{
    public class PointInfo
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Delta { get; set; }
        public double D { get; set; }
    }

    public class Segment
    {
        public PointInfo X { get; set; }
        public PointInfo Y { get; set; }

        public District District { get; set; }
    }

    public struct PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD ConverPoint(Segment segment)
        {
            return new PointD()
            {
                X = (X - segment.X.Min) * segment.X.D + segment.District.Left,
                Y = (segment.Y.Max - Y) * segment.Y.D + segment.District.Top
            };
        }

        public PointD FixPoint()
        {
            const double halfPinSize = 23.0;
            return new PointD()
            {
                X = X - halfPinSize,
                Y = Y - halfPinSize
            };
        }
    }

}