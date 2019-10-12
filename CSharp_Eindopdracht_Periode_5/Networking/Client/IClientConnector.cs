using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Client
{
    public interface IClientConnector
    {
        void OnDisconnect();
    }
}
