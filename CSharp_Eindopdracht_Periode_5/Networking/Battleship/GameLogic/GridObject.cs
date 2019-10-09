using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Networking.Battleship.GameLogic
{
    public class GridObject
    {
        private int originIndexX, originIndexY;
        private int directionX, directionY;
        private int frontSize, backSize;

        public GridObject(int size, int originIndexX, int originIndexY)
        {
            this.originIndexX = originIndexX;
            this.originIndexY = originIndexY;
            this.directionX = 0;
            this.directionY = 1;
            SetSize(size);
        }

        public void RotateRight()
        {
            int tempX = this.directionX;
            this.directionX = -this.directionY;
            this.directionY = tempX;
        }

        public void RotateLeft()
        {
            int tempX = this.directionX;
            this.directionX = this.directionY;
            this.directionY = -tempX;
        }

        public Tuple<int, int> GetIndexFront()
        {
            int frontX = ((int)this.directionX * this.frontSize) + originIndexX;
            int frontY = ((int)this.directionY * this.frontSize) + originIndexY;
            return new Tuple<int, int>(frontX, frontY);
        }

        public Tuple<int, int> GetIndexBack()
        {
            int backX = (-(int)this.directionX * this.frontSize) + originIndexX;
            int backY = (-(int)this.directionY * this.frontSize) + originIndexY;
            return new Tuple<int, int>(backX, backY);
        }

        public void SetOriginIndex(int originIndexX, int originIndexY)
        {
            this.originIndexX = originIndexX;
            this.originIndexY = originIndexY;
        }

        public void SetSize(int size)
        {
            this.frontSize = size / 2;
            this.backSize = size / 2;

            if (size % 2 == 0)
                this.backSize--;
        }
    }
}
