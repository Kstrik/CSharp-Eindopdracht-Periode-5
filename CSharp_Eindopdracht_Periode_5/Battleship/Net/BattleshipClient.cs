using Networking.Battleship;
using Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Battleship.Net
{
    public class BattleshipClient : IServerDataReceiver, IClientConnector
    {
        private Client client;
        private IServerMessageReceiver messageReceiver;

        public BattleshipClient(string ip, int port, IServerMessageReceiver messageReceiver)
        {
            this.client = new Client(ip, port, this, this, null);
            this.messageReceiver = messageReceiver;
        }

        public bool Connect()
        {
            return this.client.Connect();
        }

        public void Diconnect()
        {
            this.client.Disconnect();
        }

        public void OnDataReceived(byte[] data)
        {
            this.messageReceiver?.OnMessageReceived(Message.ParseMessage(data));
        }

        public void OnDisconnect()
        {
            MessageBox.Show("Server closed application wil be closed!");
            Environment.Exit(0);
        }

        public void Transmit(Message message)
        {
            this.client.Transmit(message.GetBytes());
        }

        public void SetMessageReceiver(IServerMessageReceiver messageReceiver)
        {
            this.messageReceiver = messageReceiver;
        }
    }
}
