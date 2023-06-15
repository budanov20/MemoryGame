using System;
using System.Drawing;

namespace MemoryGame
{
    public class Circle
    {
        public Circle(int X, int Y, int R, Color color)
        {
            this.X = X;
            this.Y = Y;
            this.R = R;
            this.color = color;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public Color color { get; set; }

        public void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(color);
            g.FillEllipse(brush, X-R, Y-R, 2*R, 2*R);
        }
        public bool Collides(Circle otherCircle)
        {
            return (Math.Sqrt(Math.Pow(this.X - otherCircle.X, 2) + Math.Pow(this.Y - otherCircle.Y, 2)) < this.R + otherCircle.R);
        }
        public bool Collides(Point point)
        {
            return (Math.Sqrt(Math.Pow(this.X - point.X, 2) + Math.Pow(this.Y - point.Y, 2)) < this.R);
        }
        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}, R: {2}", this.X, this.Y, this.R);
        }
    }
}
