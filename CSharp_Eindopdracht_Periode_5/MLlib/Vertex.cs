using MLlib.Vectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLlib
{
    public class Vertex
    {
        public Vector3D Vertice;
        public Vector2D UVCoordinate;
        public Vector3D Normal;

        public Vertex(Vector3D vertice, Vector2D uvCoordinate, Vector3D normal)
        {
            this.Vertice = vertice;
            this.UVCoordinate = uvCoordinate;
            this.Normal = normal;
        }
    }
}
