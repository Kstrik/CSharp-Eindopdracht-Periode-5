using Battleship_Server.GameLogic;
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

        private Game game;

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
                BroadcastPlayers();
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
                        playerInSession.Session = null;
                        this.battleshipServer.Transmit(new Message(Message.ID.LEAVE_SESSION, Message.State.OK, null), playerInSession.GetConnection());
                    }
                    this.players.Clear();
                }
                else
                {
                    this.players.Remove(player);
                    player.Session = null;
                    this.battleshipServer.Transmit(new Message(Message.ID.LEAVE_SESSION, Message.State.OK, null), player.GetConnection());
                    Broadcast(new Message(Message.ID.REMOVE_PLAYER, Message.State.OK, Encoding.UTF8.GetBytes(player.GetUsername())));
                    BroadcastPlayers();
                }
            }
        }

        public void HandleMessage(Message message, Player player)
        {
            List<byte> content = new List<byte>(message.GetContent());

            switch (message.GetId())
            {
                case Message.ID.CHAT_MESSAGE:
                    {
                        if(player.IsAuthorized)
                            Broadcast(new Message(Message.ID.CHAT_MESSAGE, Message.State.OK, content.ToArray()));
                        break;
                    }
                case Message.ID.GET_PLAYERS:
                    {
                        if (player.IsAuthorized && this.game == null)
                        {
                            foreach (Player playerInSession in this.players)
                                this.battleshipServer.Transmit(new Message(Message.ID.PLAYERDATA, Message.State.OK, playerInSession.GetBytes()), player.GetConnection());
                        }
                        break;
                    }
                case Message.ID.READY:
                    {
                        if (player.IsAuthorized && this.game == null)
                        {
                            player.IsReady = true;
                            this.battleshipServer.Transmit(new Message(Message.ID.READY, Message.State.OK, null), player.GetConnection());
                        }
                        break;
                    }
                case Message.ID.UNREADY:
                    {
                        if (player.IsAuthorized && this.game == null)
                        {
                            player.IsReady = false;
                            this.battleshipServer.Transmit(new Message(Message.ID.UNREADY, Message.State.OK, null), player.GetConnection());
                        }
                        break;
                    }
                case Message.ID.START_GAME:
                    {
                        if (player.IsAuthorized && this.host == player && this.game == null)
                        {
                            this.game = new Game(new GamePlayer(this.players[0]), new GamePlayer(this.players[1]), this, this.battleshipServer);
                            Broadcast(new Message(Message.ID.START_GAME, Message.State.OK, null));
                        }
                        break;
                    }
                case Message.ID.END_GAME:
                    {
                        if (player.IsAuthorized && this.game != null)
                        {
                            Broadcast(new Message(Message.ID.END_GAME, Message.State.OK, Encoding.UTF8.GetBytes(this.game.GetEndGameText())));
                            this.game = null;
                        }
                        break;
                    }
                default:
                    {
                        if (this.game != null)
                        {
                            if(!this.game.HandleMessage(message, player))
                            {
                                this.game = null;
                            }
                        }
                        break;
                    }
            }
        }

        public void SendChatMessage(string message)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)message.Length);
            bytes.AddRange(Encoding.UTF8.GetBytes(message));
            bytes.AddRange(Encoding.UTF8.GetBytes("Server"));

            Broadcast(new Message(Message.ID.CHAT_MESSAGE, Message.State.OK, bytes.ToArray()));
        }

        public void Broadcast(Message message)
        {
            foreach (Player playerInSession in this.players)
                this.battleshipServer.Transmit(message, playerInSession.GetConnection());
        }

        public void BroadcastPlayers()
        {
            foreach (Player playerInSession in this.players)
                Broadcast(new Message(Message.ID.PLAYERDATA, Message.State.OK,playerInSession.GetBytes()));
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.UTF8.GetBytes(this.SessionId + this.name));
            bytes.Add((byte)this.players.Count());

            return bytes.ToArray();
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
