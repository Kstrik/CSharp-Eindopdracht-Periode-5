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
                    case Message.ID.SESSIONDATA:
                        {
                            string sessionId = Encoding.UTF8.GetString(content.GetRange(0, 32).ToArray());
                            string sessionName = Encoding.UTF8.GetString(content.GetRange(32, content.Count - 33).ToArray());
                            int playerCount = int.Parse(Encoding.UTF8.GetString(content.GetRange(content.Count - 1, 1).ToArray()));

                            SessionListItem sessionListItem = new SessionListItem(JoinSession);
                            sessionListItem.SessionName = sessionName;
                            sessionListItem.SessionId = sessionId;
                            sessionListItem.PlayerCount = playerCount;
                            sessionListItem.Foreground = Brushes.White;
                            
                            if(con_Sessions.Children.Count == 0)
                                sessionListItem.Margin = new Thickness(5, 5, 5, 5);
                            else
                                sessionListItem.Margin = new Thickness(5, 0, 5, 5);

                            sessionListItem.InnerMargin = new Thickness(5, 5, 5, 5);
                            sessionListItem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
                            sessionListItem.ButtonTemplate = (ControlTemplate)FindResource("ButtonControlTemplate");
                            sessionListItem.ButtonStyle = (Style)FindResource("ButtonStyle");

                            SessionListItem existingControl = null;
                            foreach (SessionListItem control in con_Sessions.Children)
                            {
                                if(control.SessionId == sessionId)
                                {
                                    control.PlayerCount = playerCount;
                                    existingControl = control;
                                    break;
                                }
                            }

                            if(existingControl != null)
                                existingControl = sessionListItem;
                            else
                                con_Sessions.Children.Add(sessionListItem);
                            
                            break;
                        }
                    case Message.ID.ADD_SESSION:
                        {
                            if (message.GetState() == Message.State.OK)
                                ShowLobby(txb_SessionName.Text);
                            else if (message.GetState() == Message.State.ERROR)
                            {
                                txb_SessionName.IsEnabled = true;
                                btn_Refresh.IsEnabled = true;
                                btn_Logout.IsEnabled = true;
                                btn_HostSession.IsEnabled = true;
                                MessageBox.Show(Encoding.UTF8.GetString(content.ToArray()));
                            }
                            break;
                        }
                    case Message.ID.JOIN_SESSION:
                        {
                            if (message.GetState() == Message.State.OK)
                                ShowLobby(Encoding.UTF8.GetString(content.ToArray()));
                            else if (message.GetState() == Message.State.ERROR)
                            {
                                txb_SessionName.IsEnabled = true;
                                btn_Refresh.IsEnabled = true;
                                btn_Logout.IsEnabled = true;
                                btn_HostSession.IsEnabled = true;
                                MessageBox.Show(Encoding.UTF8.GetString(content.ToArray()));
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }));
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            con_Sessions.Children.Clear();
            this.battleshipClient.Transmit(new Message(Message.ID.GET_SESSIONS, Message.State.NONE, null));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.battleshipClient.Transmit(new Message(Message.ID.LOGOUT, Message.State.NONE, null));

            AuthorizationWindow authorizationWindow = new AuthorizationWindow(this.battleshipClient);
            authorizationWindow.Show();
            this.Close();
        }

        private void HostSession_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(txb_SessionName.Text))
            {
                this.battleshipClient.Transmit(new Message(Message.ID.ADD_SESSION, Message.State.NONE, Encoding.UTF8.GetBytes(txb_SessionName.Text)));
                txb_SessionName.IsEnabled = false;
                btn_Refresh.IsEnabled = false;
                btn_Logout.IsEnabled = false;
                btn_HostSession.IsEnabled = false;
            }
            else
                MessageBox.Show("No name specified!");
        }

        private void JoinSession(string sessionId)
        {
            this.battleshipClient.Transmit(new Message(Message.ID.JOIN_SESSION, Message.State.NONE, Encoding.UTF8.GetBytes(sessionId)));
            txb_SessionName.IsEnabled = false;
            btn_Refresh.IsEnabled = false;
            btn_Logout.IsEnabled = false;
            btn_HostSession.IsEnabled = false;
        }

        private void ShowLobby(string sessionName)
        {
            MessageBox.Show(sessionName);
        }
    }
}
