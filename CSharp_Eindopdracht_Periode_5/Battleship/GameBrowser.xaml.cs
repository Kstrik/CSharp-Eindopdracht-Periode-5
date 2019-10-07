using Battleship.Net;
using Networking.Battleship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for GameBrowser.xaml
    /// </summary>
    public partial class GameBrowser : Window, IServerMessageReceiver
    {
        private BattleshipClient battleshipClient;

        public GameBrowser(BattleshipClient battleshipClient)
        {
            InitializeComponent();

            this.battleshipClient = battleshipClient;
            this.battleshipClient.SetMessageReceiver(this);
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                List<byte> content = new List<byte>(message.GetContent());

                switch (message.GetId())
                {
                    case Message.ID.REGISTER:
                        {

                            break;
                        }
                    case Message.ID.LOGIN:
                        {

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }));
        }
    }
}
