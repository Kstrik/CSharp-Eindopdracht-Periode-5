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
        //Battleship.GameObjects.Grid grid; 

        public MainWindow()
        {
            InitializeComponent();
            //this.viewport = new Viewport3D();
            //this.Content = this.viewport;
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
            GameInput.OnMouseUp(e.ChangedButton, e.GetPosition(vwp));
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
                GameInput.OnMouseMove(e.GetPosition(vwp));
        }

        private void Ship_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Image).Name)
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
    }
}