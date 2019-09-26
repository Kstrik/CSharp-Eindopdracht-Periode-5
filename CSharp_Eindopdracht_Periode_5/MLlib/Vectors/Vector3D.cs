using System;
using System.Collections.Generic;
using System.Text;

namespace MLlib.Vectors
{
    public class Vector3D
    {
        public float X, Y, Z;

        public Vector3D(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public float Magnitute()
        {
            return (float)Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2));
        }

        public Vector3D Normalized()
        {
            float magnitute = Magnitute();
            return new Vector3D(this.X / magnitute, this.Y / magnitute, this.Z / magnitute);
        }

        public static float Dot(Vector3D a, Vector3D b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        public static Vector3D Cross(Vector3D a, Vector3D b)
        {
            return new Vector3D((a.Y * b.Z) - (a.Z * b.Y),
                                (a.Z * b.X) - (a.X * b.Z),
                                (a.X * b.Y) - (a.Y * b.X));
        }

        public static Vector3D Zero()
        {
            return new Vector3D(0, 0, 0);
        }

        public Vector3D Clone()
        {
            return new Vector3D(this.X, this.Y, this.Z);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
    }
}
