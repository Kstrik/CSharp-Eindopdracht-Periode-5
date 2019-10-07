using Networking.Battleship;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Server.Net
{
    public class BattleshipServer : IClientDataReceiver, IServerConnector
    {
        private Server server;

        private List<Session> sessions;
        private List<Player> players;

        public BattleshipServer(string ip, int port)
        {
            this.server = new Server(ip, port, this, this, null);
            this.server.Start();

            this.sessions = new List<Session>();
            this.players = new List<Player>();
        }

        public void AddSession(int maxClientCount, Player host)
        {
            if (maxClientCount != 0 && host != null)
                this.sessions.Add(new Session(maxClientCount, host, this));
        }

        public void RemoveSession(Session session)
        {
            if (session != null)
                this.sessions.Remove(session);
        }

        public void OnDataReceived(byte[] data, ClientConnection connection)
        {
            Player player = GetPlayer(connection);
            Message message = Message.ParseMessage(data);

            List<byte> content = new List<byte>(message.GetContent());

            switch(message.GetId())
            {
                case Message.ID.REGISTER:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(64, content.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());
                        bool result = player.Register(username, password);
                        if (result)
                            this.server.Transmit(new Message(Message.ID.REGISTER, Message.State.OK, null).GetBytes(), connection);
                        else
                            this.server.Transmit(new Message(Message.ID.REGISTER, Message.State.ERROR, Encoding.UTF8.GetBytes("Username is already in use!")).GetBytes(), connection);
                        break;
                    }
                case Message.ID.LOGIN:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(64, content.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());
                        bool result = player.Login(username, password);
                        if (result)
                            this.server.Transmit(new Message(Message.ID.LOGIN, Message.State.OK, null).GetBytes(), connection);
                        else
                            this.server.Transmit(new Message(Message.ID.LOGIN, Message.State.ERROR, Encoding.UTF8.GetBytes("Username or password is incorrect!")).GetBytes(), connection);
                        break;
                    }
                case Message.ID.LOGOUT:
                    {
                        if(player.IsAuthorized)
                            player.Logout();
                        break;
                    }
                case Message.ID.ADD_SESSION:
                    {
                        if (player.IsAuthorized)
                            AddSession(2, player);
                        break;
                    }
                case Message.ID.JOIN_SESSION:
                    {
                        if (player.IsAuthorized)
                        {
                            string sessionId = content.ToArray().ToString();
                            Session session = this.sessions.Where(s => s.SessionId == sessionId).First();
                            if(session != null && !session.GetPlayers().Contains(player))
                            {
                                session.JoinSession(player);
                                this.server.Transmit(new Message(Message.ID.JOIN_SESSION, Message.State.OK, null).GetBytes(), connection);
                            }
                            else
                            {
                                this.server.Transmit(new Message(Message.ID.JOIN_SESSION, Message.State.ERROR, Encoding.UTF8.GetBytes("Already in session!")).GetBytes(), connection);
                            }
                        }
                        break;
                    }
                case Message.ID.LEAVE_SESSION:
                    {
                        if (player.IsAuthorized)
                        {

                        }
                        break;
                    }
            }
        }

        public void OnClientConnected(ClientConnection connection)
        {
            this.players.Add(new Player(connection));
        }

        public void OnClientDisconnected(ClientConnection connection)
        {
            Player player = GetPlayer(connection);
            Session session = this.sessions.Where(s => s.GetPlayers().Contains(player)).First();
            session?.LeaveSession(player);

            this.players.Remove(player);
        }

        private Player GetPlayer(ClientConnection connection)
        {
            return this.players.Where(p => p.GetConnection() == connection).First();
        }
    }
}
