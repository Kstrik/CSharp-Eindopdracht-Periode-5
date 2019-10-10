using Battleship.Assets;
using Battleship.GameObjects;
using Battleship.GameObjects.Water;
using Battleship.Net;
using MLlib;
using Networking.Battleship;
using Networking.Battleship.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        private List<Ship> ships;
        private int currentShipsIndex;
        private bool isInSetup;

        private BattleshipClient battleshipClient;

        public Game(Dispatcher dispatcher, ref Viewport3D viewport, BattleshipClient battleshipClient)
        {
            this.timing = Timing.GetInstance();
            this.mainDispatcher = dispatcher;
            this.world = new World(this, ref viewport);
            this.isRunning = false;

            this.battleshipClient = battleshipClient;
            this.ships = new List<Ship>();
            this.currentShipsIndex = 0;
            this.isInSetup = true;

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

            //Ship gameObject = new Ship(this, 5);
            //gameObject.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.AircraftCarrierModel));
            //gameObject.Material = new DiffuseMaterial(Brushes.Blue);
            //this.world.AddGameObject(gameObject);

            //SetupWater();
            SetupGrids();
            //this.playerGrid.Ship = gameObject;

            SetupShips();
            this.ships[0].Scaling = new Vector3D(1, 1, 1);
            this.playerGrid.Ship = this.ships[0];

            GameInput.KeyUp += OnKeyUp;
        }

        public void SetupShips()
        {
            List<(string shippath, int size)> shipsAssets = new List<(string shippath, int size)>()
            {
                (shippath: Asset.AircraftCarrierModel, size: 5),
                (shippath: Asset.BattleShipModel, size: 4),
                (shippath: Asset.CruiserModel, size: 3),
                (shippath: Asset.SubmarineModel, size: 3),
                (shippath: Asset.DestroyerModel, size: 2)
            };

            foreach ((string shippath, int size) ship in shipsAssets)
            {
                Ship gameObject = new Ship(this, ship.size);
                gameObject.Scaling = new Vector3D(0, 0, 0);
                gameObject.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(ship.shippath));
                gameObject.Material = new DiffuseMaterial(Brushes.Blue);
                this.world.AddGameObject(gameObject);
                ships.Add(gameObject);
            }         
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
            this.enemyGrid.IsActive = false;
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.Enter)
            {
                if(this.isInSetup)
                {
                    if (this.playerGrid.GetBattleshipGrid().CheckGridObjectPlacement(this.ships[this.currentShipsIndex].GridObject))
                    {
                        currentShipsIndex++;
                        if(currentShipsIndex != this.ships.Count())
                        {
                            this.playerGrid.Ship = this.ships[this.currentShipsIndex];
                            this.playerGrid.Ship.Scaling = new Vector3D(1, 1, 1);
                        }
                    }
                    if (currentShipsIndex == this.ships.Count())
                    {
                        this.playerGrid.Ship = null;

                        List<byte> bytes = new List<byte>();

                        foreach(Ship ship in this.ships)
                        {
                            GridObject gridObject = ship.GridObject;
                            bytes.AddRange(new byte[3] { (byte)gridObject.GetOriginIndex().indexX,(byte)gridObject.GetOriginIndex().indexY, (byte)gridObject.GetDirection() });
                        }

                        this.battleshipClient.Transmit(new Message(Message.ID.SUBMIT_BOATS, Message.State.NONE, bytes.ToArray()));
                        GameInput.KeyUp -= OnKeyUp;
                    }
                }
                else
                {
                    List<byte> bytes = new List<byte>();
                    bytes.Add((byte)this.playerGrid.GetIndex().X);
                    bytes.Add((byte)this.playerGrid.GetIndex().Y);
                    bytes.AddRange(Encoding.UTF8.GetBytes(UserLogin.Username));

                    GameInput.KeyUp -= OnKeyUp;
                    this.battleshipClient.Transmit(new Message(Message.ID.SUBMIT_MOVE, Message.State.NONE, bytes.ToArray()));
                }
            }
        }

        public void HandleMessage(Message message)
        {
            List<byte> content = new List<byte>(message.GetContent());

            switch (message.GetId())
            {
                case Message.ID.START_MATCH:
                    {
                        if (message.GetState() == Message.State.OK)
                        {
                            this.isInSetup = false;
                            this.playerGrid.IsActive = false;
                            this.enemyGrid.IsActive = true;
                            GameInput.KeyUp += OnKeyUp;
                        }
                        break;
                    }
                case Message.ID.SUBMIT_MOVE:
                    {
                        if (message.GetState() == Message.State.OK)
                        {
                            int indexX = content[0];
                            int indexY = content[1];
                            bool isHit = (content[2] == 1);
                            string username = Encoding.UTF8.GetString(content.GetRange(3, content.Count - 3).ToArray());

                            Point3D position = new Point3D(0, 0, 0);
                            BattleshipGrid battleshipGrid = (UserLogin.Username == username) ? this.playerGrid.GetBattleshipGrid() : this.enemyGrid.GetBattleshipGrid();

                            position = battleshipGrid.GetWorldPosition(indexX, indexY);
                            battleshipGrid.ExecuteMove(indexX, indexY);

                            GameObject pin = new GameObject(this);
                            pin.Position = new Vector3D(position.X, position.Y, position.Z);
                            pin.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.PinModel));
                            pin.Material = new DiffuseMaterial((isHit) ? Brushes.Red : Brushes.White);
                            this.world.AddGameObject(pin);

                            if(isHit)
                                GameInput.KeyUp += OnKeyUp;
                        }
                        else if(message.GetState() == Message.State.ERROR)
                        {
                            MessageBox.Show(Encoding.UTF8.GetString(content.ToArray()));
                            GameInput.KeyUp += OnKeyUp;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
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
