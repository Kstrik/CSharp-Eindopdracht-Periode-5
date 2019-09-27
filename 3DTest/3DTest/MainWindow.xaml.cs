using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MLlib;
using System.Timers;
using System.Windows.Threading;
using MLlib.Vectors;
using System.Diagnostics;

namespace _3DTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OBJModelLoader modelLoader;
        private float angle = 40;
        private Timer timer;

        private static MeshGeometry3D mesh;

        private static Point3DCollection verticesS = new Point3DCollection();
        private static Vector3DCollection normalsS = new Vector3DCollection();
        private static PointCollection uvCoordinatesS = new PointCollection();

        private static Wave wave;

        private Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            modelLoader = new OBJModelLoader();

            this.Closing += MainWindow_Closing;

            wave = new Wave();

            stopwatch.Start();
            
            // Declare scene objects.
            Viewport3D myViewport3D = new Viewport3D();
            Model3DGroup myModel3DGroup = new Model3DGroup();
            GeometryModel3D myGeometryModel = ConvertModel(modelLoader.LoadModel(@"C:\Users\Kenley Strik\Desktop\M4.obj"));
            ModelVisual3D myModelVisual3D = new ModelVisual3D();
            // Defines the camera used to view the 3D object. In order to view the 3D object,
            // the camera must be positioned and pointed such that the object is within view 
            // of the camera.
            PerspectiveCamera myPCamera = new PerspectiveCamera();
            //myPCamera.Position = new Point3D(0, 1.5f, 10);
            //myPCamera.Position = new Point3D(0, 0.5f, 2);
            //myPCamera.Position = new Point3D(0, 2.5f, 10);
            //myPCamera.Position = new Point3D(0, 10f, 100);
            myPCamera.Position = new Point3D(0, 20f, 20);
            //myPCamera.LookDirection = new Vector3D(0, 0, -1);
            //myPCamera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -0.6, -1);
            //myPCamera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -0.4, -1);
            myPCamera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -1, -1);
            myPCamera.FieldOfView = 60;
            myViewport3D.Camera = myPCamera;

            DirectionalLight myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            //myDirectionalLight.Direction = new System.Windows.Media.Media3D.Vector3D(-0.61, -0.5, -0.61);
            myDirectionalLight.Direction = new System.Windows.Media.Media3D.Vector3D(0, -1, -1);
            myModel3DGroup.Children.Add(myDirectionalLight);

            ImageBrush colors_brush = new ImageBrush();
            colors_brush.ImageSource = new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\M4_Albedo.png", UriKind.Absolute));
            DiffuseMaterial myDiffuseMaterial = new DiffuseMaterial(colors_brush);
            myGeometryModel.Material = myDiffuseMaterial;
            myGeometryModel.BackMaterial = myDiffuseMaterial;

            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            //myAxisAngleRotation3d.Axis = new System.Windows.Media.Media3D.Vector3D(0, 3, 0);
            //myAxisAngleRotation3d.Angle = angle;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            //myGeometryModel.Transform = myRotateTransform3D;

            float time = 0;
            this.timer = new Timer(1);
            this.timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>
            {
                angle += 0.5f;
                time += 0.01f;
                if(time >= 100.0f)
                {
                    time = 0;
                }
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    for (int i = 0; i < mesh.Positions.Count; i++)
                    {
                        Vertex vertex = new Vertex(new MLlib.Vectors.Vector3D((float)verticesS[i].X, (float)verticesS[i].Y, (float)verticesS[i].Z),
                                                    new MLlib.Vectors.Vector2D((float)uvCoordinatesS[i].X, (float)uvCoordinatesS[i].Y),
                                                    new MLlib.Vectors.Vector3D((float)normalsS[i].X, (float)normalsS[i].Y, (float)normalsS[i].Z));

                        wave.CalucluateVertexPosition(vertex, time);
                        mesh.Positions[i] = new Point3D(vertex.Vertice.X, vertex.Vertice.Y, vertex.Vertice.Z);
                        //mesh.Normals[i] = new System.Windows.Media.Media3D.Vector3D(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    }

                    //myAxisAngleRotation3d.Angle = angle;
                }));
            });
            //this.timer.Start();

            //for(int i = 0; i < mesh.Positions.Count; i++)
            //{
            //    Vertex vertex = new Vertex(new MLlib.Vectors.Vector3D((float)verticesS[i].X, (float)verticesS[i].Y, (float)verticesS[i].Z),
            //                                new MLlib.Vectors.Vector2D((float)uvCoordinatesS[i].X, (float)uvCoordinatesS[i].Y),
            //                                new MLlib.Vectors.Vector3D((float)normalsS[i].X, (float)normalsS[i].Y, (float)normalsS[i].Z));

            //    wave.CalucluateVertexPosition(ref vertex, 0);
            //    mesh.Positions[i] = new Point3D(vertex.Vertice.X, vertex.Vertice.Y, vertex.Vertice.Z);
            //    mesh.Normals[i] = new System.Windows.Media.Media3D.Vector3D(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
            //}

            Water water1 = new Water(Application.Current.Dispatcher);
            Water water2 = new Water(Application.Current.Dispatcher);
            water1.GetWaterPlane().GetModel().Transform = new TranslateTransform3D(5.2, 0, 0);
            water2.GetWaterPlane().GetModel().Transform = new TranslateTransform3D(-5.2, 0, 0);

            RotateTransform3D rotateTransform = new RotateTransform3D();
            AxisAngleRotation3D axisAngleRotation = new AxisAngleRotation3D();
            axisAngleRotation.Axis = new System.Windows.Media.Media3D.Vector3D(1, 0, 0);
            axisAngleRotation.Angle = 45;
            rotateTransform.Rotation = axisAngleRotation;

            Transform3DGroup transform3DGroup = new Transform3DGroup();
            transform3DGroup.Children.Add(rotateTransform);
            //transform3DGroup.Children.Add(new TranslateTransform3D(0, 5, -10));
            //water2.GetWaterPlane().GetModel().Transform = transform3DGroup;

            myModel3DGroup.Children.Add(water1.GetWaterPlane().GetModel());
            myModel3DGroup.Children.Add(water2.GetWaterPlane().GetModel());
            //myModel3DGroup.Children.Add(myGeometryModel);

            myModelVisual3D.Content = myModel3DGroup;
            myViewport3D.Children.Add(myModelVisual3D);
            this.Content = myViewport3D;

            water1.Start();
            water2.Start();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.timer.Stop();
            this.stopwatch.Stop();
        }

        private static GeometryModel3D ConvertModel(Model model)
        {
            Point3DCollection vertices = new Point3DCollection();
            foreach (MLlib.Vectors.Vector3D vertice in model.Vertices)
            {
                vertices.Add(new Point3D(vertice.X, vertice.Y, vertice.Z));
                verticesS.Add(new Point3D(vertice.X, vertice.Y, vertice.Z));
            }

            Vector3DCollection normals = new Vector3DCollection();
            foreach (MLlib.Vectors.Vector3D normal in model.Normals)
            {
                normals.Add(new System.Windows.Media.Media3D.Vector3D(normal.X, normal.Y, normal.Z));
                normalsS.Add(new System.Windows.Media.Media3D.Vector3D(normal.X, normal.Y, normal.Z));
            }

            PointCollection uvCoordinates = new PointCollection();
            foreach (MLlib.Vectors.Vector2D uvCoordinate in model.UVCoordinates)
            {
                uvCoordinates.Add(new Point(uvCoordinate.X, uvCoordinate.Y));
                uvCoordinatesS.Add(new Point(uvCoordinate.X, uvCoordinate.Y));
            }

            Int32Collection indices = new Int32Collection();
            foreach (int indice in model.Indices)
                indices.Add(indice);

            mesh = new MeshGeometry3D();
            mesh.Positions = vertices;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvCoordinates;
            mesh.TriangleIndices = indices;

            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel.Geometry = mesh;

            return geometryModel;
        }
    }
}
