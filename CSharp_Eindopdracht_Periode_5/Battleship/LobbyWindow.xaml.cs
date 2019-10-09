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
    /// Interaction logic for LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window, IServerMessageReceiver
    {
        private BattleshipClient battleshipClient;
        private string sesionName;
        private string sessionId;

        private bool isReady;

        private List<Player> players;

        public LobbyWindow(BattleshipClient battleshipClient, string sessionName, string sessionId)
        {
            InitializeComponent();

            this.battleshipClient = battleshipClient;
            this.battleshipClient.SetMessageReceiver(this);

            this.sesionName = sessionName;
            this.sessionId = sessionId;
            con_Players.Header += this.sesionName;

            this.isReady = false;

            this.players = new List<Player>();

            this.battleshipClient.Transmit(new Message(Message.ID.GET_PLAYERS, Message.State.NONE, null));
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                List<byte> content = new List<byte>(message.GetContent());

                switch (message.GetId())
                {
                    case Message.ID.PLAYERDATA:
                        {
                            bool isHost = (content[0] == 1);
                            string username = Encoding.UTF8.GetString(content.GetRange(1, content.Count - 1).ToArray());

                            PlayerListItem playerListItem = new PlayerListItem();
                            playerListItem.Username = username;
                            playerListItem.IsHost = isHost;
                            playerListItem.Foreground = Brushes.White;

                            if (con_Players.Children.Count == 0)
                                playerListItem.Margin = new Thickness(5, 5, 5, 5);
                            else
                                playerListItem.Margin = new Thickness(5, 0, 5, 5);

                            playerListItem.InnerMargin = new Thickness(5, 5, 5, 5);
                            playerListItem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
                            playerListItem.ButtonTemplate = (ControlTemplate)FindResource("ButtonControlTemplate");
                            playerListItem.ButtonStyle = (Style)FindResource("ButtonStyle");

                            PlayerListItem existingControl = null;
                            foreach (PlayerListItem control in con_Players.Children)
                            {
                                if (control.Username == username)
                                {
                                    existingControl = control;
                                    break;
                                }
                            }

                            if (existingControl != null)
                                existingControl = playerListItem;
                            else
                            {
                                con_Players.Children.Add(playerListItem);
                                txb_Chat.Text += $"Player {username} joined the session!" + Environment.NewLine;
                                txb_Chat.ScrollToEnd();
                            }

                            break;
                        }
                    case Message.ID.READY:
                        {
                            if(message.GetState() == Message.State.OK)
                            {
                                this.isReady = true;
                                btn_ReadyToggle.Content = "Unready";
                            }
                            else if (message.GetState() == Message.State.ERROR)
                                MessageBox.Show("Could not ready up!");
                            break;
                        }
                    case Message.ID.UNREADY:
                        {
                            if (message.GetState() == Message.State.OK)
                            {
                                this.isReady = false;
                                btn_ReadyToggle.Content = "Ready";
                            }
                            else if (message.GetState() == Message.State.ERROR)
                                MessageBox.Show("Could not unready!");
                            break;
                        }
                    case Message.ID.CHAT_MESSAGE:
                        {
                            if (message.GetState() == Message.State.OK)
                            {
                                int messageLength = content[0];
                                string chatMessage = Encoding.UTF8.GetString(content.GetRange(1, messageLength).ToArray());
                                string playerName = Encoding.UTF8.GetString(content.GetRange(messageLength + 1, content.Count - (messageLength + 1)).ToArray());

                                txb_Chat.Text += $"{playerName}: {chatMessage}" + Environment.NewLine;
                                txb_Chat.ScrollToEnd();
                            }
                            else if (message.GetState() == Message.State.ERROR)
                                MessageBox.Show("Error receiving chat message!");
                            break;
                        }
                    case Message.ID.REMOVE_SESSION:
                        {
                            if (message.GetState() == Message.State.OK)
                            {
                                MessageBox.Show("Host left session!");
                                GameBrowser gameBrowser = new GameBrowser(this.battleshipClient);
                                gameBrowser.Show();
                                this.Close();
                            }
                            break;
                        }
                    case Message.ID.REMOVE_PLAYER:
                        {
                            if (message.GetState() == Message.State.OK)
                            {
                                string username = Encoding.UTF8.GetString(content.ToArray());

                                PlayerListItem existingControl = null;
                                foreach (PlayerListItem control in con_Players.Children)
                                {
                                    if (control.Username == username)
                                    {
                                        existingControl = control;
                                        break;
                                    }
                                }

                                if (existingControl != null)
                                {
                                    con_Players.Children.Remove(existingControl);
                                    txb_Chat.Text += $"Player {username} left the session!" + Environment.NewLine;
                                    txb_Chat.ScrollToEnd();
                                }
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

        private void ReadyToggle_Click(object sender, RoutedEventArgs e)
        {
            if(!this.isReady)
                this.battleshipClient.Transmit(new Message(Message.ID.READY, Message.State.NONE, null));
            else
                this.battleshipClient.Transmit(new Message(Message.ID.UNREADY, Message.State.NONE, null));
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            this.battleshipClient.Transmit(new Message(Message.ID.LEAVE_SESSION, Message.State.NONE, null));
            GameBrowser gameBrowser = new GameBrowser(this.battleshipClient);
            gameBrowser.Show();
            this.Close();
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(txb_ChatMessage.Text))
            {
                List<byte> bytes = new List<byte>();
                bytes.Add((byte)txb_ChatMessage.Text.Length);
                bytes.AddRange(Encoding.UTF8.GetBytes(txb_ChatMessage.Text));
                bytes.AddRange(Encoding.UTF8.GetBytes(UserLogin.Username));

                this.battleshipClient.Transmit(new Message(Message.ID.CHAT_MESSAGE, Message.State.NONE, bytes.ToArray()));
                txb_ChatMessage.Text = "";
            }
        }
    }
}
