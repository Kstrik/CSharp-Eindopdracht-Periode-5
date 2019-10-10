using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.GameLogic;
using Networking.Battleship.GameLogic;

namespace Battleship.GameObjects
{
    public class Ship : GameObject
    {
        public GridObject GridObject;

        public Ship(Game game, int size) 
            : base(game)
        {
            this.GridObject = new GridObject(size, 0,0);
        }
    }
}
