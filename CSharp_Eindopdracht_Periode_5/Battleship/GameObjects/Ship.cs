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
        private GridObject gridObject;

        public Ship(Game game, int size) : base(game)
        {
            this.gridObject = new GridObject(size, 0,0);
        }

        public GridObject GetGridObject()
        {
            return gridObject;
        }
    }
}
