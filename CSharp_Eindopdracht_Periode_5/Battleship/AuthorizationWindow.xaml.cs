﻿using Battleship.Net;
using Networking;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window, IServerMessageReceiver
    {
        private BattleshipClient battleshipClient;

        private ThicknessAnimation slideAnimation;
        private Storyboard storyBoard;

        private bool isRegistering;

        public AuthorizationWindow(BattleshipClient battleshipClient)
        {
            InitializeComponent();

            //if (battleshipClient == null)
            //    this.battleshipClient = new BattleshipClient(ClientSettings.Ip, ClientSettings.Port, this);
            //else
            //    this.battleshipClient.SetMessageReceiver(this);
            if (battleshipClient != null)
                this.battleshipClient?.SetMessageReceiver(this);

            this.slideAnimation = new ThicknessAnimation();
            this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            this.storyBoard = new Storyboard();
            this.storyBoard.Children.Add(this.slideAnimation);
            this.isRegistering = false;
        }

        public AuthorizationWindow()
            : this(null) { }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_Ip.Text) || !String.IsNullOrEmpty(txb_Port.Text))
            {
                this.battleshipClient = new BattleshipClient(txb_Ip.Text, int.Parse(txb_Port.Text), this);
                if (!this.battleshipClient.Connect())
                {
                    lbl_Error.Content = "Connection failed!";
                    lbl_Error.Visibility = Visibility.Visible;
                }
                else
                {
                    stk_Connect.Visibility = Visibility.Collapsed;
                    stk_Content.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbl_Error.Content = "Ip and port cannot be empty!";
                lbl_Error.Visibility = Visibility.Visible;
            }
        }

        private void ShowRegister_Click(object sender, RoutedEventArgs e)
        {
            ScrollDown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_LoginUsername.Text) && !String.IsNullOrEmpty(txb_LoginPassword.Password))
            {
                btn_Login.IsEnabled = false;
                btn_ShowRegister.IsEnabled = false;

                string usernameAndPasword = HashUtil.HashSha256(txb_LoginPassword.Password) + txb_LoginUsername.Text;
                this.battleshipClient.Transmit(new Message(Message.ID.LOGIN, Message.State.NONE, Encoding.UTF8.GetBytes(usernameAndPasword)));
            }
            else
            {
                lbl_LoginError.Content = "Input cannot be empty!";
                lbl_LoginError.Visibility = Visibility.Visible;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(txb_RegisterUsername.Text) && !String.IsNullOrEmpty(txb_RegisterPassword.Password) && !String.IsNullOrEmpty(txb_RegisterConfirmPassword.Password))
            {
                if(txb_RegisterPassword.Password == txb_RegisterConfirmPassword.Password)
                {
                    btn_Register.IsEnabled = false;
                    btn_Back.IsEnabled = false;

                    string usernameAndPasword = HashUtil.HashSha256(txb_RegisterPassword.Password) + txb_RegisterUsername.Text;
                    this.battleshipClient.Transmit(new Message(Message.ID.REGISTER, Message.State.NONE, Encoding.UTF8.GetBytes(usernameAndPasword)));
                }
                else
                {
                    lbl_RegisterError.Content = "Passwords do not match!";
                    lbl_RegisterError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbl_RegisterError.Content = "Input cannot be empty!";
                lbl_RegisterError.Visibility = Visibility.Visible;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ScrollUp();
        }

        private void ScrollUp()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (this.isRegistering)
                {
                    this.slideAnimation.From = new Thickness(0, -600, 0, 0);
                    this.slideAnimation.To = new Thickness(0, 0, 0, 0);

                    storyBoard.Begin(stk_Content);
                    this.isRegistering = false;
                }
            }));
        }

        private void ScrollDown()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!this.isRegistering)
                {
                    this.slideAnimation.From = new Thickness(0, 0, 0, 0);
                    this.slideAnimation.To = new Thickness(0, -600, 0, 0);

                    storyBoard.Begin(stk_Content);
                    this.isRegistering = true;
                }
            }));
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
                            btn_Register.IsEnabled = true;
                            btn_Back.IsEnabled = true;

                            if (message.GetState() == Message.State.OK)
                            {
                                lbl_RegisterError.Visibility = Visibility.Hidden;
                                ScrollUp();
                            }
                            else if (message.GetState() == Message.State.ERROR)
                            {
                                lbl_RegisterError.Content = Encoding.UTF8.GetString(content.ToArray());
                                lbl_RegisterError.Visibility = Visibility.Visible;
                            }
                            break;
                        }
                    case Message.ID.LOGIN:
                        {
                            btn_Login.IsEnabled = true;
                            btn_ShowRegister.IsEnabled = true;

                            if (message.GetState() == Message.State.OK)
                            {
                                UserLogin.Username = txb_LoginUsername.Text;
                                lbl_LoginError.Visibility = Visibility.Hidden;
                                GameBrowser gameBrowser = new GameBrowser(this.battleshipClient);
                                gameBrowser.Show();
                                this.Close();
                            }
                            else if (message.GetState() == Message.State.ERROR)
                            {
                                lbl_LoginError.Content = Encoding.UTF8.GetString(content.ToArray());
                                lbl_LoginError.Visibility = Visibility.Visible;
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
    }
}
