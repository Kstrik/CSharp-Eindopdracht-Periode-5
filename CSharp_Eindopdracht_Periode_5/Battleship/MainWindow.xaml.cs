using Battleship.GameLogic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServerMessageReceiver
    {
        private BattleshipClient battleshipClient;

        private Game game;

        public MainWindow(BattleshipClient battleshipClient)
        {
            InitializeComponent();

            this.battleshipClient = battleshipClient;
            this.battleshipClient.SetMessageReceiver(this);

            this.game = new Game(Application.Current.Dispatcher, ref vwp);

            //this.Closing += MainWindow_Closing;
            this.game.Start();
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;
            this.MouseMove += MainWindow_MouseMove;
            //grid = new Battleship.GameObjects.Grid(this.game);


            //this.Cursor = Cursors.None;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            GameInput.OnKeyDown(e.Key);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            GameInput.OnKeyUp(e.Key);
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameInput.OnMouseDown(e.ChangedButton, e.GetPosition(vwp));
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(!txb_ChatMessage.IsFocused)
                GameInput.OnMouseUp(e.ChangedButton, e.GetPosition(vwp));
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
                GameInput.OnMouseMove(e.GetPosition(vwp));
        }

        private void Ship_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Image).Name.ToLower())
            {
                case "carrier":
                    //load boats in
                    break;
                case "cruiser":

                    break;
                case "battleship":

                    break;
                case "submarine":

                    break;
                case "destroyer":

                    break;
                default:
                    break;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.game.Stop();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            this.game.Start();
        }

        private void StopGame_Click(object sender, RoutedEventArgs e)
        {
            this.game.Stop();
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                List<byte> content = new List<byte>(message.GetContent());

                switch (message.GetId())
                {
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
                                MessageBox.Show(Encoding.UTF8.GetString(content.ToArray()));
                            break;
                        }
                    case Message.ID.SUBMIT_BOATS:
                        {
                            if(message.GetState() == Message.State.OK)
                            {

                            }
                            break;
                        }
                    case Message.ID.START_MATCH:
                        {
                            if (message.GetState() == Message.State.OK)
                            {

                            }
                            break;
                        }
                    case Message.ID.SUBMIT_MOVE:
                        {
                            if (message.GetState() == Message.State.OK)
                            {

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

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_ChatMessage.Text))
            {
                List<byte> bytes = new List<byte>();
                bytes.Add((byte)txb_ChatMessage.Text.Length);
                bytes.AddRange(Encoding.UTF8.GetBytes(txb_ChatMessage.Text));
                bytes.AddRange(Encoding.UTF8.GetBytes(UserLogin.Username));

                this.battleshipClient.Transmit(new Message(Message.ID.CHAT_MESSAGE, Message.State.NONE, bytes.ToArray()));
                txb_ChatMessage.Text = "";
            }
        }

        private void OnKeyUp(Key key)
        {
            if(key == Key.Enter)
            {

            }
        }
    }
}