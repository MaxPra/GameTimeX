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
using System.Windows.Shapes;

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
    }
}
