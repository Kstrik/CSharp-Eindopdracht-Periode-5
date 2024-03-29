﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Networking.Battleship.GameLogic
{
    public class GridObject
    {
        public enum Direction
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3,
        }
        private Direction direction;

        private int originIndexX, originIndexY;
        private int directionX, directionY;
        private int frontSize, backSize;

        public GridObject(int size, int originIndexX, int originIndexY)
        {
            SetSize(size);
            this.originIndexX = originIndexX;
            this.originIndexY = originIndexY;
            SetDirection(Direction.DOWN);
        }

        public GridObject(int size, int originIndexX, int originIndexY, Direction direction)
        {
            SetSize(size);
            this.originIndexX = originIndexX;
            this.originIndexY = originIndexY;
            SetDirection(direction);
        }

        public void RotateRight()
        {
            int tempX = this.directionX;
            this.directionX = -this.directionY;
            this.directionY = tempX;

            if ((int)this.direction == 3)
                this.direction = Direction.UP;
            else
                this.direction = (Direction)((int)this.direction + 1);
        }

        public void RotateLeft()
        {
            int tempX = this.directionX;
            this.directionX = this.directionY;
            this.directionY = -tempX;

            if ((int)this.direction == 0)
                this.direction = Direction.LEFT;
            else
                this.direction = (Direction)((int)this.direction - 1);
        }

        public (int indexX, int indexY) GetIndexFront()
        {
            int frontX = ((int)this.directionX * this.frontSize) + originIndexX;
            int frontY = ((int)this.directionY * this.frontSize) + originIndexY;
            return (frontX, frontY);
        }

        public (int indexX, int indexY) GetIndexBack()
        {
            int backX = (-(int)this.directionX * this.backSize) + originIndexX;
            int backY = (-(int)this.directionY * this.backSize) + originIndexY;
            return (backX, backY);
        }

        public List<(int indexX, int indexY)> GetCoveredIndices()
        {
            List<(int indexX, int indexY)> indices = new List<(int indexX, int indexY)>();
            indices.Add((indexX: this.originIndexX, indexY: this.originIndexY));

            for (int i = 1; i <= this.frontSize; i++)
                indices.Add((indexX: ((int)this.directionX * i) + originIndexX, indexY: ((int)this.directionY * i) + originIndexY));
            for (int i = 1; i <= this.backSize; i++)
                indices.Add((indexX: (-(int)this.directionX * i) + originIndexX, indexY: (-(int)this.directionY * i) + originIndexY));

            return indices;
        }

        public (int indexX, int indexY) GetOriginIndex()
        {
            return (indexX: this.originIndexX, indexY: this.originIndexY);
        }

        public Direction GetDirection()
        {
            return this.direction;
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

        public void SetDirection(Direction direction)
        {
            this.direction = direction;

            switch (direction)
            {
                case Direction.UP:
                    {
                        this.directionX = 0;
                        this.directionY = -1;
                        break;
                    }
                case Direction.DOWN:
                    {
                        this.directionX = 0;
                        this.directionY = 1;
                        break;
                    }
                case Direction.LEFT:
                    {
                        this.directionX = -1;
                        this.directionY = 0;
                        break;
                    }
                case Direction.RIGHT:
                    {
                        this.directionX = 1;
                        this.directionY = 0;
                        break;
                    }
            }
        }
    }
}
