using Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Net
{
    public class BattleshipClient : IServerDataReceiver
    {
        private Client client;

        public BattleshipClient(string ip, int port)
        {
            this.client = new Client(ip, port, this, null);
        }

        public void OnDataReceived(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
