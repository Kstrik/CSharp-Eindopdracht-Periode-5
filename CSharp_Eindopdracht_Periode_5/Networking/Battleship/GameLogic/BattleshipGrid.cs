using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Networking.Battleship.GameLogic
{
    public class BattleshipGrid
    {
        private Node[,] nodes;

        private double nodeSize;
        private int sizeX, sizeY;
        private Point3D origin;

        public BattleshipGrid(double nodeSize, int sizeX, int sizeY, Point3D origin)
        {
            this.nodes = new Node[sizeX, sizeY];
            for(int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    this.nodes[x, y] = new Node();
                }
            }

            this.nodeSize = nodeSize;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.origin = origin;
        }

        public BattleshipGrid(int sizeX, int sizeY)
            : this(1, sizeX, sizeY, new System.Windows.Media.Media3D.Point3D(0, 0, 0)) { }

        public bool EvaluateMove(int indexX, int indexY)
        {
            Node test = this.nodes[indexX, indexY];
            if (this.nodes[indexX, indexY].IsHit || indexX < 0 || indexY < 0 || indexX >= this.sizeX || indexY >= this.sizeY)
                return false;
            return true;
        }

        public bool ExecuteMove(int indexX, int indexY)
        {
            this.nodes[indexX, indexY].IsHit = true;

            if (this.nodes[indexX, indexY].IsOccupied)
                return true;
            return false;
        }

        // Gets the world position of the index in the grid relative to the origin
        public Point3D GetWorldPosition(int indexX, int indexY)
        {
            if (indexX >= this.sizeX || indexY >= this.sizeY)
                return this.origin;

            double stepSizeX = this.nodeSize, stepSizeY = this.nodeSize;

            if (this.sizeX % 2 == 0)
                stepSizeX /= 2;
            if (this.sizeY % 2 == 0)
                stepSizeY /= 2;

            return new Point3D(indexX * stepSizeX, this.origin.Y, indexY * stepSizeY);
        }

        public bool CheckGridObjectPlacement(GridObject gridObject)
        {
            Tuple<int, int> frontIndex = gridObject.GetIndexFront();
            Tuple<int, int> backIndex = gridObject.GetIndexBack();

            if (frontIndex.Item1 >= 0 && frontIndex.Item1 < this.sizeX &&
                frontIndex.Item2 >= 0 && frontIndex.Item2 < this.sizeY &&
                backIndex.Item1 >= 0 && backIndex.Item1 < this.sizeX &&
                backIndex.Item2 >= 0 && backIndex.Item2 < this.sizeY)
                return true;

            return false;
        }

        public void SetOrigin(Point3D origin)
        {
            this.origin = origin;
        }
    }
}
