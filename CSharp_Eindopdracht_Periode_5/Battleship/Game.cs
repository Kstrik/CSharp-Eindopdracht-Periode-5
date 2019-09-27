using Battleship.GameObjects;
using MLlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Battleship
{
    public class Game
    {
        private Timing timing;

        private Thread gameThread;
        private Dispatcher mainDispatcher;

        private bool isRunning;

        private GameObject gameObject;
        private Viewport3D viewport;
        private Model3DGroup modelGroup;
        private ModelVisual3D modelVisual;

        private PerspectiveCamera myPCamera;

        private World world;

        public Game(Dispatcher dispatcher, Viewport3D viewport)
        {
            GameInput.KeyDown += OnKeyDown;
            GameInput.KeyUp += OnKeyUp;

            myPCamera = new PerspectiveCamera();
            myPCamera.Position = new Point3D(0, 2, 10);
            myPCamera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, -1);
            myPCamera.FieldOfView = 60;
            viewport.Camera = myPCamera;

            this.viewport = viewport;
            this.modelGroup = new Model3DGroup();
            this.modelVisual = new ModelVisual3D();
            this.modelVisual.Content = this.modelGroup;
            this.viewport.Children.Add(this.modelVisual);

            DirectionalLight myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            myDirectionalLight.Direction = new System.Windows.Media.Media3D.Vector3D(0, -1, -1);
            this.modelGroup.Children.Add(myDirectionalLight);

            this.mainDispatcher = dispatcher;
            this.timing = Timing.GetInstance();
            this.isRunning = false;

            this.gameObject = new GameObject();
            this.gameObject.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(@"C:\Users\Kenley Strik\Desktop\M4.obj"));
            this.gameObject.Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\M4_Albedo.png", UriKind.Absolute))));
            this.modelGroup.Children.Add(this.gameObject.GeometryModel);
        }

        private void InitiliazeThread()
        {
            this.gameThread = new Thread(() =>
            {
                this.timing.Reset();
                while (this.isRunning)
                {
                    Thread.Sleep(5);
                    this.timing.Update();
                    Update(this.timing.DeltaTime);
                }
            });
        }

        public void Start()
        {
            if (!this.isRunning)
            {
                InitiliazeThread();
                this.isRunning = true;
                this.gameThread.Start();
            }
        }

        public void Stop()
        {
            if (this.isRunning)
            {
                this.isRunning = false;
                this.gameThread.Abort();
            }
        }

        public void Toggle()
        {
            this.isRunning = !this.isRunning;
            if (this.isRunning)
                this.gameThread.Start();
            else
                this.gameThread.Abort();
        }

        private void Update(float deltatime)
        {
            this.mainDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                this.gameObject.Update(deltatime);

                //this.gameObject.velocity += new Vector3D(0, 0.5 * deltatime, 0);
                //this.gameObject.angle += 90 * deltatime;
            }));
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.W)
                this.gameObject.velocity = new Vector3D(0, 0, -4);
            else if (key == Key.S)
                this.gameObject.velocity = new Vector3D(0, 0, 4);
            else if (key == Key.A)
                this.gameObject.velocity = new Vector3D(-4, 0, 0);
            else if (key == Key.D)
                this.gameObject.velocity = new Vector3D(4, 0, 0);
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.W)
                this.gameObject.velocity = new Vector3D(0, 0, -4);
            else if (key == Key.S)
                this.gameObject.velocity = new Vector3D(0, 0, 4);
            else if (key == Key.A)
                this.gameObject.velocity = new Vector3D(-4, 0, 0);
            else if (key == Key.D)
                this.gameObject.velocity = new Vector3D(4, 0, 0);
        }
    }
}
