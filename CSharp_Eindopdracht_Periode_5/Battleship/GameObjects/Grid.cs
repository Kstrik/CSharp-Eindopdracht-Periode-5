using MLlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Battleship.GameObjects
{
    class Grid : GameObject
    {
        GameObject marker;
        public Grid(Game game)
            : base(game)
        {
            Position = new Vector3D(0, 0, 0);
            GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(@"C:\Users\Levi Vlasblom\Desktop\GridPlane.obj"));
            Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"C:\Users\Levi Vlasblom\Desktop\gird.png", UriKind.Absolute))));

            marker = new GameObject(game);
            marker.Scaling = new Vector3D(1, 1, 1);
            marker.Position = new Vector3D(-4.5, 0,-4.5);
            marker.GeometryModel = ModelUtil.ConvertToGeometryModel3D(new OBJModelLoader().LoadModel(@"C:\Users\Levi Vlasblom\Desktop\Highlighter.obj"));
            marker.Material = new DiffuseMaterial(Brushes.Red);
            game.GetWorld().AddGameObject(marker);
            
            GameInput.KeyUp += UpdatePlayerMark;
        }

        private void UpdatePlayerMark(Key key)
        {

            if (key == Key.Up)
                if (marker.Position.Z - 1 >= -4.5)
                    marker.Position.Z -= 1;                
            if (key == Key.Down)
                if (marker.Position.Z + 1 <= 4.5)
                    marker.Position.Z += 1;
            if (key == Key.Left)
                if (marker.Position.X - 1  >= -4.5)
                    marker.Position.X -= 1;
            if (key == Key.Right)
                if (marker.Position.X + 1 <= 4.5 )
                    marker.Position.X += 1;
        }
    }
}
