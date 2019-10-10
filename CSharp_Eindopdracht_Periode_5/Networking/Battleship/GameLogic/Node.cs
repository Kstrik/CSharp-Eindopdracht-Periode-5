using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Battleship.GameLogic
{
    public class Node
    {
        public bool IsOccupied;
        public bool IsHit;

        public Node()
        {
            this.IsOccupied = false;
            this.IsHit = false;
        }
    }
}
