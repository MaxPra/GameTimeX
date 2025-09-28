using System.Windows;

namespace GameTimeX.XApplication.SubDisplays
{
    /// <summary>
    /// Interaktionslogik für HappyHalloween.xaml
    /// </summary>
    public partial class HappyHalloween : Window
    {
        public HappyHalloween()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
