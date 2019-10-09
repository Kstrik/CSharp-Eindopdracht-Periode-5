using Networking;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Server.Net
{
    public class Player
    {
        private ClientConnection clientConnection;

        private string username;
        public Session Session { get; set; }

        public bool IsAuthorized { get { return this.isAuthorized; } }
        private bool isAuthorized;

        public Player(ClientConnection clientConnection)
        {
            this.clientConnection = clientConnection;
            this.isAuthorized = false;
        }

        public bool Register(string username, string password)
        {
            return Authorizer.AddNewAuthorization(username, password);
        }

        public bool Login(string username, string password)
        {
            if(Authorizer.CheckAuthorization(username, password))
            {
                this.username = username;
                this.isAuthorized = true;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            this.username = "";
            this.isAuthorized = false;
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)((this.Session.GetHost() == this) ? 1 : 0));
            bytes.AddRange(Encoding.UTF8.GetBytes(this.username));

            return bytes.ToArray();
        }

        public ClientConnection GetConnection()
        {
            return this.clientConnection;
        }

        public string GetUsername()
        {
            return this.username;
        }
    }
}
