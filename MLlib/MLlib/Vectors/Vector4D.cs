using System;
using System.Collections.Generic;
using System.Text;

namespace MLlib.Vectors
{
    public class Vector4D
    {
        public float X, Y, Z, W;

        public Vector4D(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vector4D Clone()
        {
            return new Vector4D(this.X, this.Y, this.Z, this.W);
        }

        public static Vector4D operator +(Vector4D a, Vector4D b)
        {
            return new Vector4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Vector4D operator -(Vector4D a, Vector4D b)
        {
            return new Vector4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }
    }
}
