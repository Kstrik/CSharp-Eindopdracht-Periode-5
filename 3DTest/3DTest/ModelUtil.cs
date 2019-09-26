using MLlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DTest
{
    public class ModelUtil
    {
        public static Tuple<MeshGeometry3D, GeometryModel3D> ConvertModel(Model model)
        {
            Point3DCollection vertices = new Point3DCollection();
            foreach (MLlib.Vectors.Vector3D vertice in model.Vertices)
                vertices.Add(new Point3D(vertice.X, vertice.Y, vertice.Z));

            Vector3DCollection normals = new Vector3DCollection();
            foreach (MLlib.Vectors.Vector3D normal in model.Normals)
                normals.Add(new System.Windows.Media.Media3D.Vector3D(normal.X, normal.Y, normal.Z));

            PointCollection uvCoordinates = new PointCollection();
            foreach (MLlib.Vectors.Vector2D uvCoordinate in model.UVCoordinates)
                uvCoordinates.Add(new Point(uvCoordinate.X, uvCoordinate.Y));

            Int32Collection indices = new Int32Collection();
            foreach (int indice in model.Indices)
                indices.Add(indice);

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = vertices;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvCoordinates;
            mesh.TriangleIndices = indices;

            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel.Geometry = mesh;

            return new Tuple<MeshGeometry3D, GeometryModel3D>(mesh, geometryModel);
        }
    }
}
