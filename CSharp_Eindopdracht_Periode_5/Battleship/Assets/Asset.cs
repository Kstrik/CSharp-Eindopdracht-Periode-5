using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Assets
{
    public static class Asset
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string assetsFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Assets");

        //Models
        public static string HighlighterModel = assetsFolderPath + @"\Models\Highlighter.obj";
        public static string GridPlaneModel = assetsFolderPath + @"\Models\GridPlane.obj";
        public static string WaterTileModel = assetsFolderPath + @"\Models\WaterTile.obj";
        public static string PinModel = assetsFolderPath + @"\Models\Pin.obj";

        public static string AircraftCarrierModel = assetsFolderPath + @"\Models\AircraftCarrier.obj";
        public static string BattleShipModel = assetsFolderPath + @"\Models\Battleship.obj";
        public static string CruiserModel = assetsFolderPath + @"\Models\Carrier.obj";
        public static string SubmarineModel = assetsFolderPath + @"\Models\Submarine.obj";
        public static string DestroyerModel = assetsFolderPath + @"\Models\Destroyer.obj";

        //Images
        public static string GridImage = assetsFolderPath + @"\Images\Grid.png";
        public static string WaterImage = assetsFolderPath + @"\Images\Water.jpg";
    }
}
