using System;
using System.Collections.Generic;
using System.Text;

namespace MLlib.Vectors
{
    public class Vector2D
    {
        public float X, Y;

        public Vector2D(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float Magnitute()
        {
            return (float)Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
        }

        public Vector2D Normalized()
        {
            float magnitute = Magnitute();
            return new Vector2D(this.X / magnitute, this.Y / magnitute);
        }

        public static float Dot(Vector2D a, Vector2D b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public static float Cross(Vector2D a, Vector2D b)
        {
            return (a.X - b.Y) + (a.Y * b.X);
        }

        public static Vector2D Zero()
        {
            return new Vector2D(0, 0);
        }


        public Vector2D Clone()
        {
            return new Vector2D(this.X, this.Y);
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }
    }
}
