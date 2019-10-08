using Battleship.GameObjects;
using Battleship.GameObjects.Water;
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

namespace Battleship.GameLogic
{
    public class Game
    {
        private Timing timing;

        private Thread gameThread;
        private Dispatcher mainDispatcher;

        private bool isRunning;

        private World world;
        private GameCamera gameCamera;

        public Game(Dispatcher dispatcher, ref Viewport3D viewport)
        {
            this.timing = Timing.GetInstance();
            this.mainDispatcher = dispatcher;
            this.world = new World(this, ref viewport);
            this.isRunning = false;

            //PerspectiveCamera camera = new PerspectiveCamera();
            //camera.Position = new Point3D(0, 2, 10);
            //camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, -1);
            //camera.FieldOfView = 60;
            this.gameCamera = new GameCamera(this);
            this.world.CurrentCamera = this.gameCamera.GetCamera();

            DirectionalLight myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            myDirectionalLight.Direction = new System.Windows.Media.Media3D.Vector3D(0, -1, -1);
            this.world.AddLight(myDirectionalLight);

            //GameObject gameObject1 = new GameObject(this);
            //gameObject1.Position = new Vector3D(1, 0, 0);
            //gameObject1.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(@"C:\Users\Kenley Strik\Desktop\cars.obj"));
            //gameObject1.Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\gencar_blue.png", UriKind.Absolute))));
            //this.world.AddGameObject(gameObject1);

            //GameObject gameObject2 = new GameObject(this);
            //gameObject2.Position = new Vector3D(0, 0, 0);
            //gameObject2.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(@"C:\Users\Kenley Strik\Desktop\cars.obj"));
            //gameObject2.Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\gencar_blue.png", UriKind.Absolute))));
            //this.world.AddGameObject(gameObject2);

            GameObjects.Grid playerGrid = new GameObjects.Grid(this);
            playerGrid.Position = new Vector3D(-10, 0, 0);
            this.world.AddGameObject(playerGrid);

            GameObjects.Grid enemyGrid = new GameObjects.Grid(this);
            playerGrid.Position = new Vector3D(10, 0, 0);
            this.world.AddGameObject(enemyGrid);

            //Water water = new Water(this);
            //this.world.AddGameObject(water);
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
            DispatchAction(new Action(() =>
            {
                this.gameCamera.Update(deltatime);
                this.world.Update(deltatime);
            }));
        }

        public void DispatchAction(Action action)
        {
            this.mainDispatcher.Invoke(DispatcherPriority.Background, action);
        }

        public World GetWorld()
        {
            return this.world;
        }
    }
}
