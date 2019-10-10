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
    /// Interaction logic for PlayerListItem.xaml
    /// </summary>
    public partial class PlayerListItem : UserControl
    {
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(PlayerListItem));
        public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(PlayerListItem));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(PlayerListItem));
        public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.Register("ButtonTemplate", typeof(ControlTemplate), typeof(PlayerListItem));

        public string Username
        {
            get { return (string)this.GetValue(UsernameProperty); }
            set { this.SetValue(UsernameProperty, value); }
        }

        public Thickness InnerMargin
        {
            get { return (Thickness)this.GetValue(InnerMarginProperty); }
            set { this.SetValue(InnerMarginProperty, value); }
        }

        public Style ButtonStyle
        {
            get { return (Style)this.GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        public ControlTemplate ButtonTemplate
        {
            get { return (ControlTemplate)this.GetValue(ButtonTemplateProperty); }
            set { this.SetValue(ButtonTemplateProperty, value); }
        }

        public bool IsHost
        {
            get
            {
                return this.isHost;
            }
            set
            {
                this.isHost = value;
                lbl_Host.Content = (this.isHost) ? "Host" : "";
            }
        }
        private bool isHost;

        public bool IsReady
        {
            get
            {
                return this.isReady;
            }
            set
            {
                this.isReady = value;
                lbl_Ready.Content = (this.isReady) ? "Ready" : "Not ready";
                lbl_Ready.Foreground = (this.isReady) ? Brushes.Green : Brushes.Red;
            }
        }
        private bool isReady;

        public PlayerListItem()
        {
            InitializeComponent();
        }
    }
}
