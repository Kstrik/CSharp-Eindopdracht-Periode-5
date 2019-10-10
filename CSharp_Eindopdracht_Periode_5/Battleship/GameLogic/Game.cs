using Battleship.Assets;
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

        private SelectionGrid playerGrid;
        private SelectionGrid enemyGrid;

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

            DirectionalLight directionalLight = new DirectionalLight();
            directionalLight.Color = Colors.White;
            directionalLight.Direction = new Vector3D(0, -1, -1);
            this.world.AddLight(directionalLight);

            GameObject gameObject = new GameObject(this);
            gameObject.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.AircraftCarrierModel));
            gameObject.Material = new DiffuseMaterial(Brushes.Blue);
            this.world.AddGameObject(gameObject);

            //SetupWater();
            SetupGrids();
            this.playerGrid.Ship = gameObject;
        }

        private void SetupWater()
        {
            Water water1 = new Water(this);
            water1.Position = new Vector3D(-5.5, -2.2, 0);
            this.world.AddGameObject(water1);
            Water water2 = new Water(this);
            water2.Position = new Vector3D(5.5, -2.2, 0);
            this.world.AddGameObject(water2);
        }

        private void SetupGrids()
        {
            this.playerGrid = new SelectionGrid(this, true);
            this.playerGrid.Position = new Vector3D(-5.5, 0, 0);
            this.playerGrid.Marker.Position = new Vector3D(this.playerGrid.Position.X - 4.5, this.playerGrid.Position.Y, this.playerGrid.Position.Z - 4.5);
            this.world.AddGameObject(playerGrid.Marker);
            this.world.AddGameObject(playerGrid);

            this.enemyGrid = new SelectionGrid(this, false);
            this.enemyGrid.Position = new Vector3D(5.5, 0, 0);
            this.enemyGrid.Marker.Position = new Vector3D(enemyGrid.Position.X - 4.5, enemyGrid.Position.Y, enemyGrid.Position.Z - 4.5);
            this.world.AddGameObject(enemyGrid.Marker);
            this.world.AddGameObject(enemyGrid);
        }

        private void OnKeyUp(Key key)
        {
            if (key == Key.Enter)
            {

            }
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
