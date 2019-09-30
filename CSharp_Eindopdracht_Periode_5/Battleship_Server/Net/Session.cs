using Networking;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Server.Net
{
    public class Session
    {
        public string SessionId { get; }
        private int maxPlayerCount;
        private List<Player> players;

        private Player host;
        private BattleshipServer battleshipServer;

        public Session(int maxPlayerCount, Player host, BattleshipServer battleshipServer)
        {
            this.SessionId = HashUtil.HashMD5(host.GetUsername());
            this.maxPlayerCount = maxPlayerCount;
            this.players = new List<Player>();

            this.host = host;
            this.battleshipServer = battleshipServer;
            this.players.Add(this.host);
        }

        public bool JoinSession(Player player)
        {
            if (player != null && this.players.Count != this.maxPlayerCount)
            {
                this.players.Add(player);
                return true;
            }
            return false;
        }

        public void LeaveSession(Player player)
        {
            if(player != null)
            {
                if (player == this.host)
                    this.battleshipServer.RemoveSession(this);
                else
                    this.players.Remove(player);
            }
        }

        public List<Player> GetPlayers()
        {
            return this.players;
        }
    }
}
