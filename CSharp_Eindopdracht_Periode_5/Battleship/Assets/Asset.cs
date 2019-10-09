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

        public static string AircraftCarrierModel = assetsFolderPath + @"\Models\AircraftCarrier.obj";

        //Images
        public static string GridImage = assetsFolderPath + @"\Images\Grid.png";
        public static string WaterImage = assetsFolderPath + @"\Images\Water.jpg";
    }
}
