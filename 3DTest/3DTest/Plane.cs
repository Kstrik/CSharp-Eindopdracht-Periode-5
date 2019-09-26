using MLlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace _3DTest
{
    public class Plane
    {
        private double width;
        private double depth;

        private int resolutionX;
        private int resolutionY;

        private GeometryModel3D model;
        private MeshGeometry3D mesh;

        public Plane(double width, double depth, int resolutionX, int resolutionY)
        {
            this.width = width;
            this.depth = depth;

            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;

            this.model = new GeometryModel3D();
            GenerateMesh();
        }

        private void GenerateMesh()
        {
            //for(int z = 0; z < this.resolutionY - 1; z++)
            //{
            //    for(int x = 0; x < this.resolutionX - 1; x++)
            //    {

            //    }
            //}
            OBJModelLoader modelLoader = new OBJModelLoader();
            Tuple<MeshGeometry3D, GeometryModel3D> modelLoadResults = ModelUtil.ConvertModel(modelLoader.LoadModel(@"C:\Users\Kenley Strik\Desktop\Tile2.obj"));
            this.model = modelLoadResults.Item2;
            this.mesh = modelLoadResults.Item1;
        }

        public GeometryModel3D GetModel()
        {
            return this.model;
        }

        public MeshGeometry3D GetMesh()
        {
            return this.mesh;
        }
    }
}
