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

        public void AddSession(int maxClientCount, Player host, string name)
        {
            if (maxClientCount != 0 && host != null)
            {
                if (this.sessions.Where(s => s.GetHost() == host).Count() == 0)
                {
                    if(this.sessions.Where(s => s.GetName() == name).Count() == 0)
                    {
                        Session session = new Session(maxClientCount, host, name, this);
                        this.sessions.Add(session);
                        host.Session = session;
                        this.Transmit(new Message(Message.ID.ADD_SESSION, Message.State.OK, null), host.GetConnection());

                        BroadcastSessions();
                    }
                    else
                        this.Transmit(new Message(Message.ID.ADD_SESSION, Message.State.ERROR, Encoding.UTF8.GetBytes("Session name is already in use!")), host.GetConnection());
                }
                else
                {
                    this.Transmit(new Message(Message.ID.ADD_SESSION, Message.State.ERROR, Encoding.UTF8.GetBytes("Your already hosting a session!")), host.GetConnection());
                }
            }
        }

        public void JoinSession(string sessionId, Player player)
        {
            Session session = this.sessions.Where(s => s.SessionId == sessionId).First();
            if (session != null && !session.GetPlayers().Contains(player))
            {
                session.JoinSession(player);

                BroadcastSessions();
            }
            else
                this.Transmit(new Message(Message.ID.JOIN_SESSION, Message.State.ERROR, Encoding.UTF8.GetBytes("Already in session!")), player.GetConnection());
        }

        public void LeaveSession(Player player)
        {
            Session session = player.Session;
            if(session != null)
            {
                player.Session.LeaveSession(player);

                if (session.GetHost() == player)
                {
                    RemoveSession(session);
                    Broadcast(new Message(Message.ID.REMOVE_SESSION, Message.State.OK, Encoding.UTF8.GetBytes(session.SessionId)));
                }

                BroadcastSessions();
            }
        }

        public void RemoveSession(Session session)
        {
            if (session != null)
                this.sessions.Remove(session);
        }

        private void BroadcastSessions()
        {
            foreach (Session session in this.sessions)
                this.Broadcast(new Message(Message.ID.SESSIONDATA, Message.State.NONE, session.GetBytes()));
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
                            this.Transmit(new Message(Message.ID.REGISTER, Message.State.OK, null), connection);
                        else
                            this.Transmit(new Message(Message.ID.REGISTER, Message.State.ERROR, Encoding.UTF8.GetBytes("Username is already in use!")), connection);
                        break;
                    }
                case Message.ID.LOGIN:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(64, content.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());
                        bool result = player.Login(username, password);
                        if (result)
                            this.Transmit(new Message(Message.ID.LOGIN, Message.State.OK, null), connection);
                        else
                            this.Transmit(new Message(Message.ID.LOGIN, Message.State.ERROR, Encoding.UTF8.GetBytes("Username or password is incorrect!")), connection);
                        break;
                    }
                case Message.ID.LOGOUT:
                    {
                        if(player.IsAuthorized)
                            player.Logout();
                        break;
                    }
                case Message.ID.GET_SESSIONS:
                    {
                        if (player.IsAuthorized)
                        {
                            foreach(Session session in this.sessions)
                                this.Transmit(new Message(Message.ID.SESSIONDATA, Message.State.NONE, session.GetBytes()), connection);
                        }
                        break;
                    }
                case Message.ID.ADD_SESSION:
                    {
                        if (player.IsAuthorized)
                            AddSession(2, player, Encoding.UTF8.GetString(content.ToArray()));
                        break;
                    }
                case Message.ID.JOIN_SESSION:
                    {
                        if (player.IsAuthorized)
                            JoinSession(Encoding.UTF8.GetString(content.ToArray()), player);
                        break;
                    }
                case Message.ID.LEAVE_SESSION:
                    {
                        if (player.IsAuthorized)
                            LeaveSession(player);
                        break;
                    }
                default:
                    {
                        player.Session.HandleMessage(message, player);
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
            player.Logout();
            LeaveSession(player);

            this.players.Remove(player);
        }

        private Player GetPlayer(ClientConnection connection)
        {
            return this.players.Where(p => p.GetConnection() == connection).First();
        }

        public void Transmit(Message message, ClientConnection connection)
        {
            this.server.Transmit(message.GetBytes(), connection);
        }

        public void Broadcast(Message message)
        {
            this.server.Broadcast(message.GetBytes());
        }
    }
}
