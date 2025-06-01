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
        private Point3D origin; // Point3D is defined in System.Windows.Media.Media3D
        private Point3D startPoint;

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

            this.startPoint = new Point3D(origin.X, origin.Y, origin.Z);
            this.startPoint.X -= (this.sizeX * this.nodeSize) / 2.0D - (this.nodeSize / 2);
            this.startPoint.Z -= (this.sizeY * this.nodeSize) / 2.0D - (this.nodeSize / 2);
        }

        public BattleshipGrid(int sizeX, int sizeY)
            : this(1, sizeX, sizeY, new Point3D(0, 0, 0)) { }

        public bool EvaluateMove(int indexX, int indexY)
        {
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

            //double stepSizeX = this.nodeSize, stepSizeY = this.nodeSize;

            //if (this.sizeX % 2 == 0)
            //    stepSizeX /= 2;
            //if (this.sizeY % 2 == 0)
            //    stepSizeY /= 2;

            //double startX = (-stepSizeX * (this.sizeX / 2)), startY = (-stepSizeY * (this.sizeY / 2));

            //double posX = this.startPoint.X + ((indexX + 1) * (this.nodeSize / 2.0D));
            //double posZ = this.startPoint.Z + ((indexY + 1) * (this.nodeSize / 2.0D));
            double posX = this.startPoint.X + (indexX * this.nodeSize);
            double posZ = this.startPoint.Z + (indexY * this.nodeSize);

            return new Point3D(posX, this.origin.Y, posZ);
        }

        public bool CheckGridObjectPlacement(GridObject gridObject)
        {
            (int indexX, int indexY) frontIndex = gridObject.GetIndexFront();
            (int indexX, int indexY) backIndex = gridObject.GetIndexBack();

            if (frontIndex.indexX < 0 || frontIndex.indexX >= this.sizeX ||
                frontIndex.indexY < 0 || frontIndex.indexY >= this.sizeY ||
                backIndex.indexX < 0 || backIndex.indexX >= this.sizeX ||
                backIndex.indexY < 0 || backIndex.indexY >= this.sizeY)
                return false;

            foreach((int indexX, int indexY) index in gridObject.GetCoveredIndices())
            {
                if (this.nodes[index.indexX, index.indexY].IsOccupied)
                    return false;
            }

            //if (frontIndex.indexX >= 0 && frontIndex.indexX < this.sizeX &&
            //    frontIndex.indexY >= 0 && frontIndex.indexY < this.sizeY &&
            //    backIndex.indexX >= 0 && backIndex.indexX < this.sizeX &&
            //    backIndex.indexY >= 0 && backIndex.indexY < this.sizeY)
            //    return true;

            return true;
        }

        public void PlaceGridObject(GridObject gridObject)
        {
            List<(int X, int Y)> indices = gridObject.GetCoveredIndices();
            foreach ((int X, int Y) index in indices)
                this.nodes[index.X, index.Y].IsOccupied = true;
        }

        public Node GetNode(int indexX, int indexY)
        {
            return this.nodes[indexX, indexY];
        }

        public void SetOrigin(Point3D origin)
        {
            this.origin = origin;
        }
    }
}
