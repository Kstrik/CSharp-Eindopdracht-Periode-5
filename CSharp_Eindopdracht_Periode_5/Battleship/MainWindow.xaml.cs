using Battleship.GameLogic;
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
        private Viewport3D viewport;
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            this.viewport = new Viewport3D();
            this.Content = this.viewport;
            this.game = new Game(Application.Current.Dispatcher, ref this.viewport);

            this.Closing += MainWindow_Closing;
            this.game.Start();
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;
            this.MouseMove += MainWindow_MouseMove;

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
            GameInput.OnMouseDown(e.ChangedButton, e.GetPosition(this.viewport));
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GameInput.OnMouseUp(e.ChangedButton, e.GetPosition(this.viewport));
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
                GameInput.OnMouseMove(e.GetPosition(this.viewport));
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
