using Battleship.GameLogic;
using MLlib;
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
    
    class Grid : GameObject
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string filesFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Assets");
        public Point Index;
        GameObject marker;

        public Grid(Game game)
            : base(game)
        {
            //string test = Battleship.Properties.Resources.GridPlane.ToString();
            //var path = Path.GetTempPath();


            Index = new Point(1, 1);
            Position = new Vector3D(0, 0, 0);
            GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(filesFolderPath + @"\GridPlane.obj"));
            Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(filesFolderPath + @"\gird.png", UriKind.Absolute))));

            marker = new GameObject(game);
            marker.Scaling = new Vector3D(1, 1, 1);
            marker.Position = new Vector3D(-4.5, 0,-4.5);
            marker.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(filesFolderPath + @"\Highlighter.obj"));
            marker.Material = new DiffuseMaterial(Brushes.Blue);
            game.GetWorld().AddGameObject(marker);
            
            GameInput.KeyUp += UpdatePlayerMark;
        }

        private void UpdatePlayerMark(Key key)
        {

            if (key == Key.Up)
                if (Index.Y - 1 >= 1)
                {
                    marker.Position.Z -= 1;
                    Index.Y -= 1;
                }
            if (key == Key.Down)
                if (Index.Y + 1 <= 10)
                {
                    marker.Position.Z += 1;
                    Index.Y += 1;
                }
                    
            if (key == Key.Left)
                if (Index.X - 1  >= 1)
                {
                    marker.Position.X -= 1;
                    Index.X -= 1;
                }
                   
            if (key == Key.Right)
                if (Index.X + 1 <= 10 )
                {
                    marker.Position.X += 1;
                    Index.X += 1;
                }
                    
        }
    }
}
