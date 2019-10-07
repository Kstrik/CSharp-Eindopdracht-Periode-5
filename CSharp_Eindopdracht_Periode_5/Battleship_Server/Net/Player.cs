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
                return true;
            }
            return false;
        }

        public void Logout()
        {
            this.isAuthorized = false;
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
