using Battleship_Server.Net;
using Networking.Battleship.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Server.GameLogic
{
    public class GamePlayer
    {
        private BattleshipGrid grid;
        public Player Player { get; }

        public int HitPoints;

        public GamePlayer(Player player)
        {
            this.grid = new BattleshipGrid(10, 10);
            this.Player = player;
            this.HitPoints = 17;
        }

        public BattleshipGrid GetGrid()
        {
            return this.grid;
        }
    }
}
