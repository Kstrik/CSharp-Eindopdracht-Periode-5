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
    public delegate void JoinEventHandler(string sessionId);

    /// <summary>
    /// Interaction logic for SessionListItem.xaml
    /// </summary>
    public partial class SessionListItem : UserControl
    {
        public static readonly DependencyProperty SessionNameProperty = DependencyProperty.Register("SessionName", typeof(string), typeof(SessionListItem));
        public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(SessionListItem));
        public static readonly DependencyProperty PlayerCountProperty = DependencyProperty.Register("PlayerCount", typeof(int), typeof(SessionListItem));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(SessionListItem));
        public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.Register("ButtonTemplate", typeof(ControlTemplate), typeof(SessionListItem));

        public string SessionName
        {
            get { return (string)this.GetValue(SessionNameProperty); }
            set { this.SetValue(SessionNameProperty, value); }
        }

        public Thickness InnerMargin
        {
            get { return (Thickness)this.GetValue(InnerMarginProperty); }
            set { this.SetValue(InnerMarginProperty, value); }
        }

        public int PlayerCount
        {
            get { return (int)this.GetValue(PlayerCountProperty); }
            set { this.SetValue(PlayerCountProperty, value); }
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

        public string SessionId;

        private JoinEventHandler joinEventHandler;

        public SessionListItem(JoinEventHandler joinEventHandler)
        {
            InitializeComponent();

            this.joinEventHandler = joinEventHandler;
        }
        public SessionListItem()
            : this(null) { }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            if (this.joinEventHandler != null)
                this.joinEventHandler(this.SessionId);
        }
    }
}
