using MousepadSimulator.Angular;
using System;

namespace MousepadSimulator
{
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int MaxX;
        public int MaxY;

        public Rectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.MaxX = width - 1;
            this.MaxY = height - 1;
        }

        public PointF Clamp(PointF dest)
        {
            if (dest.X < X) dest.X =X;
            if (dest.X > MaxX) dest.X = MaxX;
            if (dest.Y < Y) dest.Y = Y;
            if (dest.Y > MaxY) dest.Y = MaxY;
            return dest;
        }
    }
}
