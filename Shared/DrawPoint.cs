using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Shared
{
    public class DrawPoint
    {
        private double x1;
        private double x2;
        private double y1;
        private double y2;
        private Color color;
        private double thickness;

        public DrawPoint(double x1, double x2, double y1, double y2, Color color, double thickness)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            this.color = color;
            this.thickness = thickness;
        }

        public static DrawPoint CreatePointFromLine(Line line, Color color)
        {
            return new DrawPoint(line.X1, line.X2, line.Y1, line.Y2, color, line.StrokeThickness);
        }

        public double X1
        {
            get { return this.x1; }
        }

        public double X2
        {
            get { return this.x2; }
        }

        public double Y1
        {
            get { return this.y1; }
        }

        public double Y2
        {
            get { return this.y2; }
        }

        public Color Color
        {
            get { return this.color; }
        }

        public double Thickness
        {
            get { return this.thickness; }
        }
    }
}
