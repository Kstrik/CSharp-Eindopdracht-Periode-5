using Networking.Battleship;
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
        private IServerMessageReceiver messageReceiver;

        public BattleshipClient(string ip, int port, IServerMessageReceiver messageReceiver)
        {
            this.client = new Client(ip, port, this, null);
            this.messageReceiver = messageReceiver;
            this.client.Connect();
        }

        public void OnDataReceived(byte[] data)
        {
            this.messageReceiver?.OnMessageReceived(Message.ParseMessage(data));
        }

        public void Transmit(Message message)
        {
            this.client.Transmit(message.GetBytes());
        }
    }
}
