using System.Windows;
using System.Windows.Media;

namespace GameTimeX.XApplication.SubDisplays
{
    public partial class LoadingApplication : Window
    {
        public LoadingApplication()
        {
            InitializeComponent();
        }

        private const double Corner = 12.0; // identisch zum Border CornerRadius

        private void ClipHost_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyRoundClip();
        }

        private void ClipHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyRoundClip();
        }

        private void ApplyRoundClip()
        {
            if (ClipHost.ActualWidth <= 0 || ClipHost.ActualHeight <= 0) return;

            ClipHost.Clip = new RectangleGeometry(
                new Rect(0, 0, ClipHost.ActualWidth, ClipHost.ActualHeight),
                Corner, Corner);
        }

        // <summary>
        /// Setzt den Info-Text und blendet den Balken ein (oder aus, falls leer).
        /// </summary>
        public void ShowInfo(string text)
        {
            InfoText.Text = text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(InfoText.Text))
            {
                HideInfo();
                return;
            }

            InfoPanel.Visibility = Visibility.Visible;
            InfoPanel.BeginAnimation(OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(1, new Duration(System.TimeSpan.FromMilliseconds(180))));
        }

        /// <summary>
        /// Blendet den Info-Balken aus und leert den Text.
        /// </summary>
        public void HideInfo()
        {
            InfoPanel.BeginAnimation(OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0, new Duration(System.TimeSpan.FromMilliseconds(120))));
            InfoPanel.Visibility = Visibility.Collapsed;
            InfoText.Text = string.Empty;
        }
    }
}
