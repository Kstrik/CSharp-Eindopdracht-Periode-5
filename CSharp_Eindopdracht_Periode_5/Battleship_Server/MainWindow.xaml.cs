using Battleship_Server.Net;
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

namespace Battleship_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BattleshipServer battleshipServer;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.battleshipServer?.Stop();
            Environment.Exit(0);
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if(button.Content.ToString() == "Start")
            {
                if(!String.IsNullOrEmpty(txb_Ip.Text) || !String.IsNullOrEmpty(txb_Port.Text))
                {
                    this.battleshipServer = new BattleshipServer(txb_Ip.Text, int.Parse(txb_Port.Text));
                    this.battleshipServer.Start();

                    txb_Ip.IsEnabled = false;
                    txb_Port.IsEnabled = false;
                    button.Content = "Stop";
                    lbl_Error.Visibility = Visibility.Hidden;
                }
                else
                {
                    lbl_Error.Content = "Ip and port cannot be empty!";
                    lbl_Error.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.battleshipServer.Stop();
                this.battleshipServer = null;
                txb_Ip.IsEnabled = true;
                txb_Port.IsEnabled = true;
                button.Content = "Start";
            }
        }
    }
}
