﻿using Battleship.Assets;
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
        private bool isHost;

        public Game(Dispatcher dispatcher, ref Viewport3D viewport, BattleshipClient battleshipClient, bool isHost)
        {
            this.timing = Timing.GetInstance();
            this.mainDispatcher = dispatcher;
            this.world = new World(this, ref viewport);
            this.isRunning = false;

            this.battleshipClient = battleshipClient;
            this.isHost = isHost;
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

            SetupWater();
            SetupGrids();

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
            this.playerGrid = new SelectionGrid(this, true, new Vector3D(-5.5, 0, 0));
            //this.playerGrid.Position = new Vector3D(-5.5, 0, 0);
            //this.playerGrid.Marker.Position = new Vector3D(this.playerGrid.Position.X - 4.5, this.playerGrid.Position.Y, this.playerGrid.Position.Z - 4.5);
            this.world.AddGameObject(playerGrid.Marker);
            this.world.AddGameObject(playerGrid);

            this.enemyGrid = new SelectionGrid(this, false, new Vector3D(5.5, 0, 0));
            //this.enemyGrid.Position = new Vector3D(5.5, 0, 0);
            //this.enemyGrid.Marker.Position = new Vector3D(this.enemyGrid.Position.X - 4.5, this.enemyGrid.Position.Y, this.enemyGrid.Position.Z - 4.5);
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
                        this.playerGrid.GetBattleshipGrid().PlaceGridObject(this.playerGrid.Ship.GridObject);
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

                    this.battleshipClient.Transmit(new Message(Message.ID.SUBMIT_MOVE, Message.State.NONE, bytes.ToArray()));
                    GameInput.KeyUp -= OnKeyUp;
                }
            }
        }

        public void StartMatch()
        {
            this.isInSetup = false;
            this.playerGrid.IsActive = false;
            this.enemyGrid.IsActive = true;

            if (this.isHost)
                GameInput.KeyUp += OnKeyUp;
        }

        public void SubmitMove(int indexX, int indexY, bool isHit, string username)
        {
            Point3D position = new Point3D(0, 0, 0);
            SelectionGrid selectionGrid = (UserLogin.Username != username) ? this.playerGrid : this.enemyGrid;

            position = selectionGrid.GetBattleshipGrid().GetWorldPosition(indexX, indexY);
            selectionGrid.GetBattleshipGrid().ExecuteMove(indexX, indexY);
            selectionGrid.UpdateMarkerMaterial();

            GameObject pin = new GameObject(this);
            pin.Position = new Vector3D(position.X, position.Y, position.Z) + new Vector3D(0, 0.25, 0);
            pin.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.PinModel));
            pin.Material = new DiffuseMaterial((isHit) ? Brushes.Red : Brushes.White);

            this.world.RemoveGameObject(selectionGrid.Marker);
            this.world.RemoveGameObject(selectionGrid);
            this.world.AddGameObject(pin);
            this.world.AddGameObject(selectionGrid.Marker);
            this.world.AddGameObject(selectionGrid);

            if (isHit && UserLogin.Username == username || !isHit && UserLogin.Username != username)
                GameInput.KeyUp += OnKeyUp;
        }

        public void OnSubmitMoveFailed()
        {
            GameInput.KeyUp += OnKeyUp;
        }

        public void ActivateGridControls()
        {
            this.playerGrid.ActivateControls();
            this.enemyGrid.ActivateControls();
        }

        public void ReleaseGridControls()
        {
            this.playerGrid.ReleaseControls();
            this.enemyGrid.ReleaseControls();
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

        public GameCamera GetGameCamera()
        {
            return this.gameCamera;
        }
    }
}
