using Networking.Battleship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Net
{
    public interface IServerMessageReceiver
    {
        void OnMessageReceived(Message message);
    }
}
