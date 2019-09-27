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
            this.game = new Game(Application.Current.Dispatcher, ref viewport);

            this.Closing += MainWindow_Closing;
            this.game.Start();
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            GameInput.OnKeyDown(e.Key);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            GameInput.OnKeyUp(e.Key);
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
    }
}
