using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GameTimeX
{
    /// <summary>
    /// Interaktionslogik für CreateNew.xaml
    /// </summary>
    public partial class InfoBox : Window
    {
        SolidColorBrush normalButtonColor = Brushes.CornflowerBlue;
        SolidColorBrush hoverColor = Brushes.MediumBlue;

        public int Pid { get; set; }

        private string infoText = "";

        public InfoBox(string infoText)
        {
            InitializeComponent();

            this.infoText = infoText;
        }

        public InfoBox(string infoText, bool showButton)
        {
            InitializeComponent();

            this.infoText = infoText;

            if (!showButton)
            {
                this.btnOK.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Event wird aufgerufen, wenn linker Mousebutton geklickt wird
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblInfoText.Text = infoText;
        }
    }
}
