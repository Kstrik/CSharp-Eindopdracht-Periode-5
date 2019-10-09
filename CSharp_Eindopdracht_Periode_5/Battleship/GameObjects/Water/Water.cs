using Battleship.Assets;
using Battleship.GameLogic;
using MLlib;
using MLlib.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Battleship.GameObjects.Water
{
    public class Water : GameObject
    {
        private Vector4D waveA;
        private Vector4D waveB;
        private Vector4D waveC;

        private System.Windows.Media.Media3D.Point3DCollection orignalPositions;
        private System.Windows.Media.Media3D.Vector3DCollection orignalNormals;

        private System.Windows.Media.Media3D.MeshGeometry3D mesh;

        private float time;
        private int vertexesCount;

        int frameCounter = 0;

        public Water(Game game) 
            : base(game)
        {
            //this.waveA = new Vector4D(0.5f, 0.0f, 0.25f, 1f);
            //this.waveB = new Vector4D(0.0f, 0.5f, 0.12f, 2.5f);
            //this.waveC = new Vector4D(0.5f, 0.5f, 0.12f, 1f);
            this.waveA = new Vector4D(0.5f, 0.0f, 0.25f, 1f);
            this.waveB = new Vector4D(0.0f, 1.0f, 0.12f, 2.5f);
            this.waveC = new Vector4D(0.5f, 1.0f, 0.12f, 1f);

            this.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.WaterTileModel));
            this.Material = new System.Windows.Media.Media3D.DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Asset.WaterImage, UriKind.Absolute))));

            this.orignalPositions = new System.Windows.Media.Media3D.Point3DCollection();
            this.orignalNormals = new System.Windows.Media.Media3D.Vector3DCollection();
            this.mesh = (System.Windows.Media.Media3D.MeshGeometry3D)this.GeometryModel.Geometry;
            GatherOriginalVertexData();

            this.time = 0;
        }

        private void GatherOriginalVertexData()
        {
            for (int i = 0; i < this.mesh.Positions.Count; i++)
            {
                this.orignalPositions.Add(new System.Windows.Media.Media3D.Point3D(this.mesh.Positions[i].X, this.mesh.Positions[i].Y, this.mesh.Positions[i].Z));
                this.orignalNormals.Add(new System.Windows.Media.Media3D.Vector3D(this.mesh.Normals[i].X, this.mesh.Normals[i].Y, this.mesh.Normals[i].Z));
            }
            this.vertexesCount = this.mesh.Positions.Count;
        }

        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            this.time += deltatime / 16;
            if (this.time >= 100.0f)
            {
                this.time = 0;
            }

            this.frameCounter++;
            if (this.frameCounter == 17)
                this.frameCounter = 0;

            if(this.frameCounter == 16)
            {
                for (int i = 0; i < this.vertexesCount; i++)
                {
                    Vertex vertex = new Vertex(new Vector3D((float)this.orignalPositions[i].X, (float)this.orignalPositions[i].Y, (float)this.orignalPositions[i].Z),
                                                null,
                                                new Vector3D((float)this.orignalNormals[i].X, (float)this.orignalNormals[i].Y, (float)this.orignalNormals[i].Z));

                    CalucluateVertexPosition(vertex, time);
                    this.mesh.Positions[i] = new System.Windows.Media.Media3D.Point3D(vertex.Vertice.X, vertex.Vertice.Y, vertex.Vertice.Z);
                    //this.waterPlane.GetMesh().Normals[i] = new System.Windows.Media.Media3D.Vector3D(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                }
            }
        }

        private void CalucluateVertexPosition(Vertex vertexData, float time)
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
