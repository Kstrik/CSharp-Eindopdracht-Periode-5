using MLlib.Vectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLlib
{
    public class Model
    {
        public string Name;
        public List<Vector3D> Vertices;
        public List<Vector2D> UVCoordinates;
        public List<Vector3D> Normals;
        public List<int> Indices;

        public Model()
        {
            this.Vertices = new List<Vector3D>();
            this.UVCoordinates = new List<Vector2D>();
            this.Normals = new List<Vector3D>();
            this.Indices = new List<int>();
        }
    }
}
