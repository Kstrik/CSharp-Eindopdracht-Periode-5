using MLlib;
using MLlib.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Timers;

namespace _3DTest
{
    public class Water
    {
        //private Vector4D waveA = new Vector4D(1.0f, 0.0f, 0.5f, 10.0f);
        //private Vector4D waveB = new Vector4D(0.0f, 1.0f, 0.25f, 20.0f);
        //private Vector4D waveC = new Vector4D(1.0f, 1.0f, 0.15f, 10.0f);
        private Vector4D waveA = new Vector4D(0.5f, 0.0f, 0.25f, 2f);
        private Vector4D waveB = new Vector4D(0.0f, 0.5f, 0.12f, 5f);
        private Vector4D waveC = new Vector4D(0.5f, 0.5f, 0.12f, 2f);

        private Plane waterPlane;

        private System.Windows.Media.Media3D.Point3DCollection orignalPositions;
        private System.Windows.Media.Media3D.Vector3DCollection orignalNormals;

        private System.Timers.Timer timer;

        private bool isRunning;

        private float time;
        private int vertexesCount;

        private Dispatcher dispatcher;

        public Water(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.waterPlane = new Plane(10, 10, 10, 10);

            this.orignalPositions = new System.Windows.Media.Media3D.Point3DCollection();
            this.orignalNormals = new System.Windows.Media.Media3D.Vector3DCollection();
            GatherOriginalVertexData();

            this.isRunning = false;

            InitializeTimer();

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\1.jpg", UriKind.Absolute));
            System.Windows.Media.Media3D.DiffuseMaterial waterMaterial = new System.Windows.Media.Media3D.DiffuseMaterial(brush);
            this.waterPlane.GetModel().Material = waterMaterial;
            this.waterPlane.GetModel().BackMaterial = waterMaterial;
        }

        private void GatherOriginalVertexData()
        {
            for(int i = 0; i < this.waterPlane.GetMesh().Positions.Count; i++)
            {
                this.orignalPositions.Add(new System.Windows.Media.Media3D.Point3D(this.waterPlane.GetMesh().Positions[i].X, this.waterPlane.GetMesh().Positions[i].Y, this.waterPlane.GetMesh().Positions[i].Z));
                this.orignalNormals.Add(new System.Windows.Media.Media3D.Vector3D(this.waterPlane.GetMesh().Normals[i].X, this.waterPlane.GetMesh().Normals[i].Y, this.waterPlane.GetMesh().Normals[i].Z));
            }
            this.vertexesCount = this.waterPlane.GetMesh().Positions.Count;
        }

        private void InitializeTimer()
        {
            this.timer = new System.Timers.Timer(1);
            this.timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>
            {
                this.dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    for (int i = 0; i < this.vertexesCount; i++)
                    {
                        Vertex vertex = new Vertex(new Vector3D((float)this.orignalPositions[i].X, (float)this.orignalPositions[i].Y, (float)this.orignalPositions[i].Z),
                                                    null,
                                                    new Vector3D((float)this.orignalNormals[i].X, (float)this.orignalNormals[i].Y, (float)this.orignalNormals[i].Z));

                        CalucluateVertexPosition(vertex, time);
                        this.waterPlane.GetMesh().Positions[i] = new System.Windows.Media.Media3D.Point3D(vertex.Vertice.X, vertex.Vertice.Y, vertex.Vertice.Z);
                        //this.waterPlane.GetMesh().Normals[i] = new System.Windows.Media.Media3D.Vector3D(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    }

                    UpdateTime();

                }));
            });
        }

        public void Start()
        {
            this.isRunning = true;
            this.timer.Start();
        }

        public void Stop()
        {
            this.isRunning = false;
            this.timer.Stop();
        }

        private void UpdateTime()
        {
            this.time += 0.01f;
            if (this.time >= 100.0f)
            {
                this.time = 0;
            }
        }

        //private void ChangePosition(int index, float time)
        //{
        //    this.dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
        //    {
        //        Vertex vertex = new Vertex(new Vector3D((float)this.orignalPositions[index].X, (float)this.orignalPositions[index].Y, (float)this.orignalPositions[index].Z),
        //        null,
        //        new Vector3D((float)this.orignalNormals[index].X, (float)this.orignalNormals[index].Y, (float)this.orignalNormals[index].Z));

        //        CalucluateVertexPosition(vertex, time);
        //        this.waterPlane.GetMesh().Positions[index] = new System.Windows.Media.Media3D.Point3D(vertex.Vertice.X, vertex.Vertice.Y, vertex.Vertice.Z);
        //    }));
        //}

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

        public Plane GetWaterPlane()
        {
            return this.waterPlane;
        }
    }
}
