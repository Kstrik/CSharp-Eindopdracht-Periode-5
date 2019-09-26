using MLlib;
using MLlib.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DTest
{
    public class Wave
    {
        private Vector4D waveA;
        private Vector4D waveB;
        private Vector4D waveC;

        public Wave()
        {
            //this.waveA = new Vector4D(1.0f, 0.0f, 0.5f, 10.0f);
            //this.waveB = new Vector4D(0.0f, 1.0f, 0.25f, 20.0f);
            //this.waveC = new Vector4D(1.0f, 1.0f, 0.15f, 10.0f);
            //this.waveA = new Vector4D(1.0f, 0.0f, 0.5f, 1f);
            //this.waveB = new Vector4D(0.0f, 1.0f, 0.25f, 5f);
            //this.waveC = new Vector4D(1.0f, 1.0f, 0.15f, 1f);
            this.waveA = new Vector4D(0.5f, 0.0f, 0.25f, 2f);
            this.waveB = new Vector4D(0.0f, 0.5f, 0.12f, 5f);
            this.waveC = new Vector4D(0.5f, 0.5f, 0.12f, 2f);
        }

        public void CalucluateVertexPosition(Vertex vertexData, float time)
        {
            Vector3D gridPoint = vertexData.Vertice;
            Vector3D tangent = Vector3D.Zero();
            Vector3D binormal = Vector3D.Zero();
            Vector3D p = gridPoint;
            p += GerstnerWave(this.waveA, gridPoint, tangent, binormal, time);
            p += GerstnerWave(this.waveB, gridPoint, tangent, binormal, time);
            p += GerstnerWave(this.waveC, gridPoint, tangent, binormal, time);
            Vector3D normal = Vector3D.Cross(binormal, tangent).Normalized();
            vertexData.Vertice = p;
            vertexData.Normal = normal;
        }

        private Vector3D GerstnerWave(Vector4D wave, Vector3D p, Vector3D tangent, Vector3D binormal, float time /* Time since level load */)
        {
            float steepness = wave.Z;
            float wavelength = wave.W;
            float k = 2 * (float)(Math.PI / wavelength);
            float c = (float)Math.Sqrt(9.8 / k);
            Vector2D d = new Vector2D(wave.X, wave.Y).Normalized();
            float f = k * (Vector2D.Dot(d, new Vector2D(p.X, p.Z)) - c * time);
            float a = steepness / k;

            //p.x += d.x * (a * cos(f));
            //p.y = a * sin(f);
            //p.z += d.y * (a * cos(f));

            tangent += new Vector3D(
                (float)(-d.X * d.X * (steepness * Math.Sin(f))),
                (float)(d.X * (steepness * Math.Cos(f))),
                (float)(-d.X * d.Y * (steepness * Math.Sin(f)))
            );
            binormal += new Vector3D(
                (float)(-d.X * d.Y * (steepness * Math.Sin(f))),
                (float)(d.Y * (steepness * Math.Cos(f))),
                (float)(-d.Y * d.Y * (steepness * Math.Sin(f)))
            );
            return new Vector3D(
                (float)(d.X * (a * Math.Cos(f))),
                (float)(a * Math.Sin(f)),
                (float)(d.Y * (a * Math.Cos(f)))
            );
        }
    }
}
