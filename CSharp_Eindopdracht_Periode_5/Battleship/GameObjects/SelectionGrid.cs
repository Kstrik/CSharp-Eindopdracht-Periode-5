using Battleship.Assets;
using Battleship.GameLogic;
using MLlib;
using Networking.Battleship.GameLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Battleship.GameObjects
{
    
    public class SelectionGrid : GameObject
    {
        private BattleshipGrid battleshipGrid;
        //private GridObject gridObject;

        private Point index;
        public GameObject Marker;

        public Ship Ship
        {
            get { return this.ship; }
            set { SetShip(value); }
        }
        private Ship ship;

        public bool IsActive
        {
            get { return this.isActive; }
            set { SetActive(value); }
        }

        private bool isActive;
        private bool isFriendly;

        public SelectionGrid(Game game, bool isFriendly, Vector3D position)
            : base(game)
        {
            this.Position = position;
            this.battleshipGrid = new BattleshipGrid(1, 10, 10, new Point3D(this.Position.X, this.Position.Y, this.Position.Z));
            //this.gridObject = new GridObject(1, 0, 0);

            this.index = new Point(0, 0);
            this.isFriendly = isFriendly;

            this.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.GridPlaneModel));
            this.Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Asset.GridImage, UriKind.Absolute))));

            this.Marker = new GameObject(game);
            this.Marker.Position = new Vector3D(this.Position.X - 4.5, this.Position.Y, this.Position.Z - 4.5);
            this.Marker.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(Asset.HighlighterModel));
            this.Marker.Material = new DiffuseMaterial(Brushes.Blue);

            this.Ship = new Ship(game, 1);

            ActivateControls();
            SetActive(true);
        }

        private void OnKeyUp(Key key)
        {
            switch(key)
            {
                case Key.Up:
                    {
                        if (this.index.Y > 0)
                        {
                            this.Marker.Position.Z -= 1;
                            this.index.Y -= 1;
                        }
                        break;
                    }
                case Key.Down:
                    {
                        if (this.index.Y < 9)
                        {
                            this.Marker.Position.Z += 1;
                            this.index.Y += 1;
                        }
                        break;
                    }
                case Key.Left:
                    {
                        if (this.index.X > 0)
                        {
                            this.Marker.Position.X -= 1;
                            this.index.X -= 1;
                        }
                        break;
                    }
                case Key.Right:
                    {
                        if (this.index.X < 9)
                        {
                            this.Marker.Position.X += 1;
                            this.index.X += 1;
                        }
                        break;
                    }
                case Key.E:
                    {
                        if (this.isFriendly && this.ship != null)
                        {
                            this.Ship.GridObject.RotateRight();
                            if (this.Ship != null)
                                this.Ship.Angle -= 90;
                        }
                        break;
                    }
                case Key.Q:
                    {
                        if (this.isFriendly && this.ship != null)
                        {
                            this.Ship.GridObject.RotateLeft();
                            if (this.Ship != null)
                                this.Ship.Angle += 90;
                        }
                        break;
                    }
            }

            //if(this.isFriendly)
            //{
            //    this.Ship.GridObject.SetOriginIndex((int)this.index.X, (int)this.index.Y);
            //    if (this.battleshipGrid.CheckGridObjectPlacement(this.Ship.GridObject))
            //        this.Marker.Material = new DiffuseMaterial(Brushes.Blue);
            //    else
            //        this.Marker.Material = new DiffuseMaterial(Brushes.Red);

            //    if(this.Ship != null)
            //    {
            //        this.Ship.Position = this.Marker.Position + new Vector3D(0, 0, 0);
            //        this.Ship.Material = this.Marker.Material;
            //    }
            //}
            if (this.isFriendly && this.Ship != null)
                CheckPlacement();
            else
                UpdateMarkerMaterial();
        }

        public void CheckPlacement()
        {
            this.Ship.GridObject.SetOriginIndex((int)this.index.X, (int)this.index.Y);
            if (this.battleshipGrid.CheckGridObjectPlacement(this.Ship.GridObject))
                this.Marker.Material = new DiffuseMaterial(Brushes.Blue);
            else
                this.Marker.Material = new DiffuseMaterial(Brushes.Red);

            this.Ship.Position = this.Marker.Position + new Vector3D(0, 0, 0);
            this.Ship.Material = this.Marker.Material;
        }

        public void UpdateMarkerMaterial()
        {
            if (this.battleshipGrid.EvaluateMove((int)this.index.X, (int)this.index.Y))
                this.Marker.Material = new DiffuseMaterial(Brushes.Blue);
            else
                this.Marker.Material = new DiffuseMaterial(Brushes.Red);
        }

        public void ActivateControls()
        {
            GameInput.KeyUp += OnKeyUp;
        }

        public void ReleaseControls()
        {
            GameInput.KeyUp -= OnKeyUp;
        }

        public Point GetIndex()
        {
            return this.index;
        }

        public BattleshipGrid GetBattleshipGrid()
        {
            return this.battleshipGrid;
        }

        private void SetActive(bool value)
        {
            this.isActive = value;

            if (this.isActive)
                this.Marker.Scaling = new Vector3D(1, 1, 1);
            else
                this.Marker.Scaling = new Vector3D(0, 0, 0);
        }

        private void SetShip(Ship ship)
        {
            this.ship = ship;
            if(ship != null)
            {
                //this.ship.GridObject.SetOriginIndex((int)this.index.X, (int)this.index.Y);
                //this.ship.Position = this.Marker.Position;
                CheckPlacement();
            }
        }
    }
}
