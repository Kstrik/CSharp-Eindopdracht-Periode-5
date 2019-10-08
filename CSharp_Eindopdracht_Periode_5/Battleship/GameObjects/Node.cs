using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.GameObjects
{
    class Node
    {
        bool isOccupied;
        bool isHit;
        Point position;

        public Node(bool isOccupied, bool isHit, Point position)
        {
            this.isOccupied = isOccupied;
            this.isHit = isHit;
            this.position = position;
        }
    }
}
