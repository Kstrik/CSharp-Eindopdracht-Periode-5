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

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            Viewport3D viewport = new Viewport3D();
            this.Content = viewport;
            this.game = new Game(Application.Current.Dispatcher, null, viewport);

            this.Closing += MainWindow_Closing;
            this.game.Start();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.game.Stop();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            this.game.Start();
            //txb_Textbox.Text = "Game started!";
        }

        private void StopGame_Click(object sender, RoutedEventArgs e)
        {
            this.game.Stop();
            //txb_Textbox.Text = "Game stoped!";
        }
    }
}
