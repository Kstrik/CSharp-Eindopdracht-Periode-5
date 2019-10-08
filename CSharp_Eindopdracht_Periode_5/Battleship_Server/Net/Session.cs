using Networking;
using Networking.Battleship;
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
        private string name;
        private BattleshipServer battleshipServer;

        public Session(int maxPlayerCount, Player host, string name, BattleshipServer battleshipServer)
        {
            this.SessionId = HashUtil.HashMD5(host.GetUsername());
            this.maxPlayerCount = maxPlayerCount;
            this.players = new List<Player>();

            this.host = host;
            this.name = name;
            this.battleshipServer = battleshipServer;
            this.players.Add(this.host);
        }

        public bool JoinSession(Player player)
        {
            if (player != null && this.players.Count != this.maxPlayerCount)
            {
                this.players.Add(player);
                player.Session = this;
                this.battleshipServer.Transmit(new Message(Message.ID.JOIN_SESSION, Message.State.OK, Encoding.UTF8.GetBytes(this.name)), player.GetConnection());
                return true;
            }
            this.battleshipServer.Transmit(new Message(Message.ID.JOIN_SESSION, Message.State.ERROR, Encoding.UTF8.GetBytes("Session is full!")), player.GetConnection());
            return false;
        }

        public void LeaveSession(Player player)
        {
            if(player != null)
            {
                if (player == this.host)
                {
                    foreach(Player playerInSession in this.players)
                    {
                        this.players.Remove(playerInSession);
                        playerInSession.Session = null;
                        this.battleshipServer.Transmit(new Message(Message.ID.LEAVE_SESSION, Message.State.OK, null), playerInSession.GetConnection());
                    }
                }
                else
                {
                    this.players.Remove(player);
                    player.Session = null;
                    this.battleshipServer.Transmit(new Message(Message.ID.LEAVE_SESSION, Message.State.OK, null), player.GetConnection());
                }
            }
        }

        public List<Player> GetPlayers()
        {
            return this.players;
        }

        public Player GetHost()
        {
            return this.host;
        }

        public string GetName()
        {
            return this.name;
        }
    }
}
